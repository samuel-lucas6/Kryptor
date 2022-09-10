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
using ChaCha20Poly1305 = Geralt.ChaCha20Poly1305;

namespace Kryptor;

public static class DecryptFile
{
    public static void Decrypt(FileStream inputFile, string outputFilePath, Span<byte> wrappedFileKeys, Span<byte> fileKey)
    {
        try
        {
            Span<byte> nonce = stackalloc byte[ChaCha20Poly1305.NonceSize]; nonce.Clear();
            Span<byte> header = DecryptHeader(inputFile, nonce, fileKey, wrappedFileKeys);
            long plaintextLength = BinaryPrimitives.ReadInt64LittleEndian(header[..Constants.Int64BytesLength]);
            Span<byte> fileName = stackalloc byte[Constants.FileNameHeaderLength];
            header[Constants.Int64BytesLength..Constants.FileNameHeaderLength].CopyTo(fileName);
            int fileNameLength = Padding.GetUnpaddedLength(fileName, fileName.Length);
            bool isDirectory = BitConverter.ToBoolean(header[^Constants.BoolBytesLength..]);
            CryptographicOperations.ZeroMemory(header);
            
            using (var outputFile = new FileStream(outputFilePath, FileHandling.GetFileStreamWriteOptions(inputFile.Length - Constants.FileHeadersLength)))
            {
                ConstantTime.Increment(nonce[..^1]);
                DecryptChunks(inputFile, outputFile, plaintextLength, nonce, fileKey);
                CryptographicOperations.ZeroMemory(fileKey);
            }
            inputFile.Dispose();
            
            if (fileNameLength > 0) {
                outputFilePath = FileHandling.RenameFile(outputFilePath, Encoding.UTF8.GetString(fileName[..fileNameLength]));
            }
            if (isDirectory) {
                FileHandling.ExtractZipFile(outputFilePath);
            }
            if (Globals.Overwrite) {
                FileHandling.DeleteFile(inputFile.Name);
            }
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            CryptographicOperations.ZeroMemory(fileKey);
            if (ex is not ArgumentException) {
                FileHandling.DeleteFile(outputFilePath);
            }
            throw;
        }
    }

    private static Span<byte> DecryptHeader(FileStream inputFile, Span<byte> nonce, Span<byte> fileKey, Span<byte> associatedData)
    {
        try
        {
            Span<byte> ciphertextHeader = stackalloc byte[Constants.EncryptedHeaderLength];
            inputFile.Read(ciphertextHeader);

            Span<byte> plaintextHeader = GC.AllocateArray<byte>(ciphertextHeader.Length - ChaCha20Poly1305.TagSize - kcChaCha20Poly1305.CommitmentSize, pinned: true);
            kcChaCha20Poly1305.Decrypt(plaintextHeader, ciphertextHeader, nonce, fileKey, associatedData);
            return plaintextHeader;
        }
        catch (CryptographicException ex)
        {
            throw new ArgumentException("Incorrect password/key, or this file has been tampered with.", ex);
        }
    }

    private static void DecryptChunks(Stream inputFile, Stream outputFile, long plaintextLength, Span<byte> nonce, Span<byte> fileKey)
    {
        Span<byte> ciphertextChunk = new byte[Constants.CiphertextChunkSize];
        Span<byte> plaintextChunk = new byte[Constants.FileChunkSize];
        Span<byte> counter = nonce[..^1];
        int bytesRead;
        while ((bytesRead = inputFile.Read(ciphertextChunk)) > 0) {
            if (inputFile.Position == inputFile.Length) {
                nonce[^1] = 1;
            }
            if (bytesRead < ciphertextChunk.Length) {
                Span<byte> ciphertext = ciphertextChunk[..bytesRead];
                Span<byte> plaintext = plaintextChunk[..(ciphertext.Length - ChaCha20Poly1305.TagSize)];
                ChaCha20Poly1305.Decrypt(plaintext, ciphertext, nonce, fileKey);
                outputFile.Write(plaintext);
                break;
            }
            ChaCha20Poly1305.Decrypt(plaintextChunk, ciphertextChunk, nonce, fileKey);
            ConstantTime.Increment(counter);
            outputFile.Write(plaintextChunk);
        }
        outputFile.SetLength(plaintextLength);
    }
}