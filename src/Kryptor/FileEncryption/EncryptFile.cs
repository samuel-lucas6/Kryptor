/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2025 Samuel Lucas

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
using ChaCha20Poly1305 = Geralt.ChaCha20Poly1305;
using Geralt;
using kcAEAD;

namespace Kryptor;

public static class EncryptFile
{
    public static void Encrypt(string inputFilePath, string outputFilePath, bool isDirectory, Span<byte> unencryptedHeader, Span<byte> wrappedFileKeys, Span<byte> fileKey)
    {
        try
        {
            using (var inputFile = new FileStream(inputFilePath, FileHandling.GetFileStreamReadOptions(inputFilePath)))
            {
                long paddedLength = inputFile.Length + GetRandomPaddingLength(inputFile.Length);
                long chunkCount = (paddedLength + Constants.FileChunkSize - 1) / Constants.FileChunkSize;
                long lastChunkPadding = paddedLength % Constants.FileChunkSize;
                if (lastChunkPadding == 0) {
                    lastChunkPadding = Constants.FileChunkSize;
                }
                long payloadLength = chunkCount * Constants.CiphertextChunkSize - (Constants.FileChunkSize - lastChunkPadding);

                using var outputFile = new FileStream(outputFilePath, FileHandling.GetFileStreamWriteOptions(Constants.FileHeadersLength + payloadLength));
                Span<byte> nonce = stackalloc byte[ChaCha20.NonceSize]; nonce.Clear();
                Span<byte> encryptedHeader = EncryptFileHeader(inputFile.Length, Path.GetFileName(inputFilePath), isDirectory, nonce, fileKey, wrappedFileKeys);
                outputFile.Write(unencryptedHeader);
                outputFile.Write(wrappedFileKeys);
                outputFile.Write(encryptedHeader);

                ConstantTime.Increment(nonce[..^1]);
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
            CryptographicOperations.ZeroMemory(fileKey);
            FileHandling.DeleteFile(outputFilePath);
            throw;
        }
    }

    private static long GetRandomPaddingLength(long unpaddedLength, double proportion = 0.10)
    {
        // The randomised padding scheme from Covert Encryption: https://github.com/samuel-lucas6/CovertPadding
        long fixedPadding = Math.Max(0, (int)(proportion * 500) - unpaddedLength);
        double effectiveSize = 200 + 1e8 * Math.Log(1 + 1e-8 * (unpaddedLength + fixedPadding));
        Span<byte> randomBytes = stackalloc byte[Constants.Int64BytesLength];
        SecureRandom.Fill(randomBytes);
        uint random1 = BinaryPrimitives.ReadUInt32LittleEndian(randomBytes[..4]);
        uint random2 = BinaryPrimitives.ReadUInt32LittleEndian(randomBytes[4..]);
        double coefficient = Math.Log(Math.Pow(2, 32)) - Math.Log(random1 + random2 * Math.Pow(2, -32) + Math.Pow(2, -33));
        return fixedPadding + (long)Math.Round(coefficient * proportion * effectiveSize);
    }

    private static Span<byte> EncryptFileHeader(long fileLength, string fileName, bool isDirectory, Span<byte> nonce, Span<byte> fileKey, Span<byte> associatedData)
    {
        Span<byte> plaintextLength = stackalloc byte[Constants.Int64BytesLength];
        BinaryPrimitives.WriteInt64LittleEndian(plaintextLength, fileLength);
        Span<byte> paddedFileName = GetPaddedFileName(fileName);
        Span<byte> spare = stackalloc byte[Constants.SpareHeaderLength]; spare.Clear();
        Span<byte> directory = BitConverter.GetBytes(isDirectory);
        Span<byte> plaintextHeader = stackalloc byte[Constants.EncryptedHeaderLength - Poly1305.TagSize - kcChaCha20Poly1305.CommitmentSize];
        Spans.Concat(plaintextHeader, plaintextLength, paddedFileName, spare, directory);

        Span<byte> ciphertextHeader = new byte[Constants.EncryptedHeaderLength];
        kcChaCha20Poly1305.Encrypt(ciphertextHeader, plaintextHeader, nonce, fileKey, associatedData);
        CryptographicOperations.ZeroMemory(plaintextHeader);
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
        if (bytesEncoded > paddedFileName.Length - 1) {
            throw new ArgumentException("The encoded file name is too long to be stored. Please rename the file.");
        }
        Padding.Pad(paddedFileName, fileNameBytes[..bytesEncoded], paddedFileName.Length);
        return paddedFileName;
    }

    private static void EncryptChunks(Stream inputFile, Stream outputFile, int chunkCount, int lastChunkPadding, Span<byte> nonce, Span<byte> fileKey)
    {
        Span<byte> plaintextChunk = GC.AllocateArray<byte>(Constants.FileChunkSize, pinned: true);
        Span<byte> ciphertextChunk = new byte[Constants.CiphertextChunkSize];
        Span<byte> counter = nonce[..^1];
        int bytesRead = 1;
        for (int i = 1; i <= chunkCount; i++) {
            if (bytesRead != 0) {
                bytesRead = inputFile.Read(plaintextChunk);
            }
            if (i == chunkCount) {
                Span<byte> plaintext = plaintextChunk[..lastChunkPadding];
                Span<byte> ciphertext = ciphertextChunk[..(plaintext.Length + Poly1305.TagSize)];
                nonce[^1] = 1;
                ChaCha20Poly1305.Encrypt(ciphertext, plaintext, nonce, fileKey);
                outputFile.Write(ciphertext);
                break;
            }
            ChaCha20Poly1305.Encrypt(ciphertextChunk, plaintextChunk, nonce, fileKey);
            ConstantTime.Increment(counter);
            outputFile.Write(ciphertextChunk);
        }
        CryptographicOperations.ZeroMemory(plaintextChunk);
    }
}
