/*
    Kryptor: A simple, modern, and secure encryption tool.
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

namespace KryptorCLI;

public static class DecryptFile
{
    public static void Decrypt(FileStream inputFile, string outputFilePath, byte[] ephemeralPublicKey, byte[] keyEncryptionKey)
    {
        var dataEncryptionKey = new byte[Constants.EncryptionKeyLength];
        try
        {
            byte[] encryptedHeader = FileHeaders.ReadEncryptedHeader(inputFile);
            byte[] nonce = FileHeaders.ReadNonce(inputFile);
            byte[] fileHeader = DecryptFileHeader(inputFile, ephemeralPublicKey, encryptedHeader, nonce, keyEncryptionKey);
            int lastChunkLength = BitConversion.ToInt32(Arrays.Copy(fileHeader, sourceIndex: 0, Constants.IntBitConverterLength));
            int fileNameLength = BitConversion.ToInt32(Arrays.Copy(fileHeader, Constants.IntBitConverterLength, Constants.IntBitConverterLength));
            dataEncryptionKey = Arrays.Copy(fileHeader, fileHeader.Length - dataEncryptionKey.Length, Constants.EncryptionKeyLength);
            CryptographicOperations.ZeroMemory(fileHeader);
            using (var outputFile = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan))
            {
                nonce = Utilities.Increment(nonce);
                byte[] additionalData = Arrays.Copy(encryptedHeader, encryptedHeader.Length - Constants.TagLength, Constants.TagLength);
                DecryptChunks(inputFile, outputFile, nonce, dataEncryptionKey, additionalData, lastChunkLength);
            }
            inputFile.Dispose();
            Globals.SuccessfulCount += 1;
            RestoreFileName.RenameFile(outputFilePath, fileNameLength);
            FileHandling.DeleteFile(inputFile.Name);
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            CryptographicOperations.ZeroMemory(dataEncryptionKey);
            if (ex is not ArgumentException) { FileHandling.DeleteFile(outputFilePath); }
            throw;
        }
    }

    private static byte[] DecryptFileHeader(FileStream inputFile, byte[] ephemeralPublicKey, byte[] encryptedFileHeader, byte[] nonce, byte[] keyEncryptionKey)
    {
        try
        {
            byte[] ciphertextLength = BitConversion.GetBytes(inputFile.Length - Constants.FileHeadersLength);
            byte[] magicBytes = FileHandling.ReadFileHeader(inputFile, offset: 0, Constants.KryptorMagicBytes.Length);
            byte[] formatVersion = FileHandling.ReadFileHeader(inputFile, Constants.KryptorMagicBytes.Length, Constants.EncryptionVersion.Length);
            FileHeaders.ValidateFormatVersion(formatVersion, Constants.EncryptionVersion);
            byte[] additionalData = Arrays.Concat(ciphertextLength, magicBytes, formatVersion, ephemeralPublicKey);
            return XChaCha20BLAKE2b.Decrypt(encryptedFileHeader, nonce, keyEncryptionKey, additionalData);
        }
        catch (CryptographicException ex)
        {
            throw new ArgumentException("Incorrect password/key, or this file has been tampered with.", ex);
        }
    }

    private static void DecryptChunks(FileStream inputFile, FileStream outputFile, byte[] nonce, byte[] dataEncryptionKey, byte[] additionalData, int lastChunkLength)
    {
        var ciphertextChunk = new byte[Constants.CiphertextChunkLength];
        inputFile.Seek(Constants.FileHeadersLength, SeekOrigin.Begin);
        while (inputFile.Read(ciphertextChunk, offset: 0, ciphertextChunk.Length) > 0)
        {
            byte[] plaintextChunk = XChaCha20BLAKE2b.Decrypt(ciphertextChunk, nonce, dataEncryptionKey, additionalData);
            nonce = Utilities.Increment(nonce);
            additionalData = Arrays.Copy(ciphertextChunk, ciphertextChunk.Length - Constants.TagLength, Constants.TagLength);
            outputFile.Write(plaintextChunk, offset: 0, plaintextChunk.Length);
        }
        outputFile.SetLength(outputFile.Length - Constants.FileChunkSize + lastChunkLength);
        CryptographicOperations.ZeroMemory(dataEncryptionKey);
    }
}