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
using cAEAD;

namespace Kryptor;

public static class DecryptFile
{
    public static void Decrypt(FileStream inputFile, string outputFilePath, Span<byte> unencryptedHeaders, Span<byte> nonce, Span<byte> headerKey)
    {
        Span<byte> fileKey = stackalloc byte[ChaCha20.KeySize];
        try
        {
            Span<byte> header = DecryptHeader(inputFile, unencryptedHeaders, nonce, headerKey);
            header[..fileKey.Length].CopyTo(fileKey);
            long plaintextLength = BinaryPrimitives.ReadInt64LittleEndian(header.Slice(fileKey.Length, Constants.LongBytesLength));
            Span<byte> fileName = stackalloc byte[Constants.FileNameHeaderLength];
            header.Slice(fileKey.Length + Constants.LongBytesLength, Constants.FileNameHeaderLength).CopyTo(fileName);
            int fileNameLength = Padding.GetUnpaddedLength(fileName, fileName.Length);
            bool isDirectory = BitConverter.ToBoolean(header[^Constants.BoolBytesLength..]);
            CryptographicOperations.ZeroMemory(header);
            using (var outputFile = new FileStream(outputFilePath, FileHandling.GetFileStreamWriteOptions(inputFile.Length - Constants.FileHeadersLength)))
            {
                ConstantTime.Increment(nonce);
                DecryptChunks(inputFile, outputFile, plaintextLength, nonce, fileKey);
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
            CryptographicOperations.ZeroMemory(headerKey);
            CryptographicOperations.ZeroMemory(fileKey);
            CryptographicOperations.ZeroMemory(nonce);
            if (ex is not ArgumentException) {
                FileHandling.DeleteFile(outputFilePath);
            }
            throw;
        }
    }

    private static Span<byte> DecryptHeader(FileStream inputFile, Span<byte> unencryptedHeaders, Span<byte> nonce, Span<byte> headerKey)
    {
        try
        {
            Span<byte> ciphertextHeader = stackalloc byte[Constants.EncryptedHeaderLength];
            inputFile.Read(ciphertextHeader);
            Span<byte> ciphertextLength = stackalloc byte[Constants.LongBytesLength];
            BinaryPrimitives.WriteInt64LittleEndian(ciphertextLength, inputFile.Length - Constants.FileHeadersLength);
            Span<byte> associatedData = stackalloc byte[ciphertextLength.Length + unencryptedHeaders.Length];
            Spans.Concat(associatedData, ciphertextLength, unencryptedHeaders);
            Span<byte> plaintextHeader = GC.AllocateArray<byte>(ciphertextHeader.Length - BLAKE2b.TagSize, pinned: true);
            ChaCha20BLAKE2b.Decrypt(plaintextHeader, ciphertextHeader, nonce, headerKey, associatedData);
            CryptographicOperations.ZeroMemory(headerKey);
            return plaintextHeader;
        }
        catch (CryptographicException ex)
        {
            throw new ArgumentException("Incorrect password/key, or this file has been tampered with.", ex);
        }
    }

    private static void DecryptChunks(Stream inputFile, Stream outputFile, long plaintextLength, Span<byte> nonce, Span<byte> fileKey)
    {
        Span<byte> ciphertextChunk = new byte[Constants.CiphertextChunkLength];
        Span<byte> plaintextChunk = new byte[Constants.FileChunkSize];
        while (inputFile.Read(ciphertextChunk) > 0) {
            ChaCha20BLAKE2b.Decrypt(plaintextChunk, ciphertextChunk, nonce, fileKey);
            ConstantTime.Increment(nonce);
            outputFile.Write(plaintextChunk);
        }
        outputFile.SetLength(plaintextLength);
        CryptographicOperations.ZeroMemory(fileKey);
        CryptographicOperations.ZeroMemory(nonce);
    }
}