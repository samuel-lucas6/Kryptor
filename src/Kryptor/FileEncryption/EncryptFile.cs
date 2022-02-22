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
using System.Security.Cryptography;
using Sodium;
using ChaCha20BLAKE2;

namespace Kryptor;

public static class EncryptFile
{
    public static void Encrypt(string inputFilePath, string outputFilePath, byte[] ephemeralPublicKey, byte[] salt, byte[] keyEncryptionKey)
    {
        var dataEncryptionKey = SodiumCore.GetRandomBytes(Constants.EncryptionKeyLength);
        try
        {
            bool zeroByteFile = FileHandling.GetFileLength(inputFilePath) == 0;
            if (zeroByteFile) { FileHandling.AppendFileName(inputFilePath); }
            using (var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan))
            using (var outputFile = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan))
            {
                var nonce = SodiumCore.GetRandomBytes(Constants.XChaChaNonceLength);
                byte[] encryptedHeader = EncryptFileHeader(inputFilePath, zeroByteFile, ephemeralPublicKey, dataEncryptionKey, nonce, keyEncryptionKey);
                FileHeaders.WriteHeaders(outputFile, ephemeralPublicKey, salt, nonce, encryptedHeader);
                nonce = Utilities.Increment(nonce);
                EncryptChunks(inputFile, outputFile, nonce, dataEncryptionKey);
            }
            Globals.SuccessfulCount += 1;
            if (Globals.Overwrite)
            {
                FileHandling.OverwriteFile(inputFilePath, outputFilePath);
            }
            else if (Globals.EncryptFileNames || zeroByteFile)
            {
                RestoreFileName.RemoveAppendedFileName(inputFilePath);
            }
            FileHandling.SetFileAttributesReadOnly(outputFilePath);
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            CryptographicOperations.ZeroMemory(dataEncryptionKey);
            FileHandling.DeleteFile(outputFilePath);
            throw;
        }
    }

    private static byte[] EncryptFileHeader(string inputFilePath, bool zeroByteFile, byte[] ephemeralPublicKey, byte[] dataEncryptionKey, byte[] nonce, byte[] keyEncryptionKey)
    {
        long fileLength = FileHandling.GetFileLength(inputFilePath);
        byte[] lastChunkLength = BitConversion.GetBytes(Convert.ToInt32(fileLength % Constants.FileChunkSize));
        byte[] fileNameLength = FileHeaders.GetFileNameLength(inputFilePath, zeroByteFile);
        byte[] fileHeader = Arrays.Concat(lastChunkLength, fileNameLength, dataEncryptionKey);
        long chunkCount = (long)Math.Ceiling((double)fileLength / Constants.FileChunkSize);
        byte[] ciphertextLength = BitConversion.GetBytes(chunkCount * Constants.CiphertextChunkLength);
        byte[] additionalData = Arrays.Concat(ciphertextLength, Constants.KryptorMagicBytes, Constants.EncryptionVersion, ephemeralPublicKey);
        return XChaCha20BLAKE2b.Encrypt(fileHeader, nonce, keyEncryptionKey, additionalData);
    }

    private static void EncryptChunks(Stream inputFile, Stream outputFile, byte[] nonce, byte[] dataEncryptionKey)
    {
        var plaintextChunk = new byte[Constants.FileChunkSize];
        while (inputFile.Read(plaintextChunk, offset: 0, plaintextChunk.Length) > 0)
        {
            byte[] ciphertextChunk = XChaCha20BLAKE2b.Encrypt(plaintextChunk, nonce, dataEncryptionKey);
            nonce = Utilities.Increment(nonce);
            outputFile.Write(ciphertextChunk, offset: 0, ciphertextChunk.Length);
        }
        CryptographicOperations.ZeroMemory(dataEncryptionKey);
    }
}