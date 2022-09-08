/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2022 Samuel Lucas

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.IO;
using System.Text;
using System.Buffers.Binary;
using System.Security.Cryptography;
using Geralt;
using kcAEAD;
using Padme;
using ChaCha20Poly1305 = Geralt.ChaCha20Poly1305;

namespace Kryptor;

public static class EncryptFile
{
    public static void Encrypt(string inputFilePath, string outputFilePath, bool isDirectory, Span<byte> unencryptedHeaders, Span<byte> headerKey)
    {
        Span<byte> fileKey = stackalloc byte[ChaCha20Poly1305.KeySize];
        SecureRandom.Fill(fileKey);
        try
        {
            using (var inputFile = new FileStream(inputFilePath, FileHandling.GetFileStreamReadOptions(inputFilePath)))
            {
                long paddedLength = inputFile.Length < 9 ? 10 : (long)PADME.GetPaddedLength((ulong)inputFile.Length);
                long chunkCount = (paddedLength + Constants.FileChunkSize - 1) / Constants.FileChunkSize;
                long lastChunkPadding = paddedLength % Constants.FileChunkSize;
                if (lastChunkPadding == 0) {
                    lastChunkPadding = Constants.FileChunkSize;
                }
                long payloadLength = chunkCount * Constants.CiphertextChunkSize - (Constants.FileChunkSize - lastChunkPadding);
                
                using var outputFile = new FileStream(outputFilePath, FileHandling.GetFileStreamWriteOptions(payloadLength + Constants.FileHeadersLength));
                Span<byte> nonce = stackalloc byte[ChaCha20Poly1305.NonceSize]; nonce.Clear();
                Span<byte> encryptedHeader = EncryptFileHeader(payloadLength, fileKey, inputFile.Length, Path.GetFileName(inputFilePath), isDirectory, nonce, headerKey);
                outputFile.Write(unencryptedHeaders);
                outputFile.Write(encryptedHeader);
                
                ConstantTime.Increment(nonce);
                EncryptChunks(inputFile, outputFile, (int)chunkCount, (int)lastChunkPadding, nonce, fileKey);
                CryptographicOperations.ZeroMemory(fileKey);
            }
            if (Globals.Overwrite) {
                FileHandling.OverwriteFile(inputFilePath, outputFilePath);
            }
            else if (isDirectory) {
                FileHandling.DeleteFile(inputFilePath);
            }
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            CryptographicOperations.ZeroMemory(headerKey);
            CryptographicOperations.ZeroMemory(fileKey);
            FileHandling.DeleteFile(outputFilePath);
            throw;
        }
    }
    
    private static Span<byte> EncryptFileHeader(long payloadLength, Span<byte> fileKey, long fileLength, string fileName, bool isDirectory, Span<byte> nonce, Span<byte> headerKey)
    {
        Span<byte> associatedData = stackalloc byte[Constants.Int64BytesLength];
        BinaryPrimitives.WriteInt64LittleEndian(associatedData, payloadLength);

        Span<byte> plaintextLength = stackalloc byte[Constants.Int64BytesLength];
        BinaryPrimitives.WriteInt64LittleEndian(plaintextLength, fileLength);
        Span<byte> paddedFileName = GetPaddedFileName(fileName);
        Span<byte> spare = stackalloc byte[Constants.Int64BytesLength * 4]; spare.Clear();
        Span<byte> directory = BitConverter.GetBytes(isDirectory);
        Span<byte> plaintextHeader = stackalloc byte[Constants.EncryptedHeaderLength - ChaCha20Poly1305.TagSize - kcChaCha20Poly1305.CommitmentSize];
        Spans.Concat(plaintextHeader, fileKey, plaintextLength, paddedFileName, spare, directory);
        
        Span<byte> ciphertextHeader = new byte[Constants.EncryptedHeaderLength];
        kcChaCha20Poly1305.Encrypt(ciphertextHeader, plaintextHeader, nonce, headerKey, associatedData);
        CryptographicOperations.ZeroMemory(plaintextHeader);
        CryptographicOperations.ZeroMemory(headerKey);
        return ciphertextHeader;
    }

    private static Span<byte> GetPaddedFileName(string fileName)
    {
        Span<byte> paddedFileName = new byte[Constants.FileNameHeaderLength];
        if (!Globals.EncryptFileNames) {
            Padding.Fill(paddedFileName);
            return paddedFileName;
        }
        
        Span<byte> fileNameBytes = new byte[Encoding.UTF8.GetMaxByteCount(fileName.Length)];
        int bytesEncoded = Encoding.UTF8.GetBytes(fileName, fileNameBytes);
        if (bytesEncoded > paddedFileName.Length) {
            throw new ArgumentException("The encoded file name is too long to be stored. Please rename the file.");
        }
        Padding.Pad(paddedFileName, fileNameBytes[..bytesEncoded], paddedFileName.Length);
        return paddedFileName;
    }

    private static void EncryptChunks(Stream inputFile, Stream outputFile, int chunkCount, int lastChunkPadding, Span<byte> nonce, Span<byte> fileKey)
    {
        Span<byte> plaintextChunk = GC.AllocateArray<byte>(Constants.FileChunkSize, pinned: true);
        Span<byte> ciphertextChunk = new byte[Constants.CiphertextChunkSize];
        
        int bytesRead, chunksRead = 0;
        while ((bytesRead = inputFile.Read(plaintextChunk)) == plaintextChunk.Length) {
            ChaCha20Poly1305.Encrypt(ciphertextChunk, plaintextChunk, nonce, fileKey);
            ConstantTime.Increment(nonce);
            outputFile.Write(ciphertextChunk);
            chunksRead++;
        }
        
        CryptographicOperations.ZeroMemory(plaintextChunk[bytesRead..]);
        for (int i = chunksRead; i < chunkCount; i++) {
            if (i == chunkCount - 1) {
                Span<byte> plaintext = plaintextChunk[..lastChunkPadding];
                Span<byte> ciphertext = ciphertextChunk[..(plaintext.Length + ChaCha20Poly1305.TagSize)];
                ChaCha20Poly1305.Encrypt(ciphertext, plaintext, nonce, fileKey);
                outputFile.Write(ciphertext);
                CryptographicOperations.ZeroMemory(plaintextChunk[..bytesRead]);
                return;
            }
            ChaCha20Poly1305.Encrypt(ciphertextChunk, plaintextChunk, nonce, fileKey);
            ConstantTime.Increment(nonce);
            outputFile.Write(ciphertextChunk);
            if (i == chunksRead) {
                CryptographicOperations.ZeroMemory(plaintextChunk[..bytesRead]);
            }
        }
    }
}