using System;
using System.IO;
using System.Security.Cryptography;
using Sodium;
using ChaCha20BLAKE2;

/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2021 Samuel Lucas

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

namespace KryptorCLI
{
    public static class DecryptFile
    {
        public static void Initialize(FileStream inputFile, string outputFilePath, byte[] ephemeralPublicKey, byte[] keyEncryptionKey)
        {
            var dataEncryptionKey = new byte[Constants.EncryptionKeyLength];
            try
            {
                byte[] encryptedHeader = FileHeaders.ReadEncryptedHeader(inputFile);
                byte[] nonce = FileHeaders.ReadNonce(inputFile);
                byte[] header = DecryptFileHeader(inputFile, ephemeralPublicKey, encryptedHeader, nonce, keyEncryptionKey);
                if (header == null) { throw new ArgumentException("Incorrect password/key, or this file has been tampered with."); }
                int lastChunkLength = FileHeaders.GetLastChunkLength(header);
                int fileNameLength = FileHeaders.GetFileNameLength(header);
                dataEncryptionKey = FileHeaders.GetDataEncryptionKey(header);
                CryptographicOperations.ZeroMemory(header);
                using (var outputFile = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan))
                {
                    nonce = Utilities.Increment(nonce);
                    byte[] additionalData = Arrays.Copy(encryptedHeader, encryptedHeader.Length - Constants.TagLength, Constants.TagLength);
                    Decrypt(inputFile, outputFile, nonce, dataEncryptionKey, additionalData, lastChunkLength);
                }
                string inputFilePath = inputFile.Name;
                inputFile.Dispose();
                Finalize(inputFilePath, outputFilePath, fileNameLength);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                CryptographicOperations.ZeroMemory(dataEncryptionKey);
                if (ex is not ArgumentException) { FileHandling.DeleteFile(outputFilePath); }
                throw;
            }
        }

        private static byte[] DecryptFileHeader(FileStream inputFile, byte[] ephemeralPublicKey, byte[] encryptedHeader, byte[] nonce, byte[] keyEncryptionKey)
        {
            byte[] additionalData = HeaderEncryption.GetAdditionalData(inputFile, ephemeralPublicKey);
            return HeaderEncryption.Decrypt(encryptedHeader, nonce, keyEncryptionKey, additionalData);
        }

        private static void Decrypt(FileStream inputFile, FileStream outputFile, byte[] nonce, byte[] dataEncryptionKey, byte[] additionalData, int lastChunkLength)
        {
            const int offset = 0;
            byte[] ciphertextChunk = new byte[Constants.TotalChunkLength];
            inputFile.Seek(Constants.FileHeadersLength, SeekOrigin.Begin);
            while (inputFile.Read(ciphertextChunk, offset, ciphertextChunk.Length) > 0)
            {
                byte[] plaintextChunk = XChaCha20BLAKE2b.Decrypt(ciphertextChunk, nonce, dataEncryptionKey, additionalData, TagLength.Medium);
                nonce = Utilities.Increment(nonce);
                additionalData = Arrays.Copy(ciphertextChunk, ciphertextChunk.Length - Constants.TagLength, Constants.TagLength);
                outputFile.Write(plaintextChunk, offset, plaintextChunk.Length);
            }
            outputFile.SetLength(outputFile.Length - Constants.FileChunkSize + lastChunkLength);
            CryptographicOperations.ZeroMemory(dataEncryptionKey);
        }

        private static void Finalize(string inputFilePath, string outputFilePath, int fileNameLength)
        {
            Globals.SuccessfulCount += 1;
            RestoreFileName.RenameFile(outputFilePath, fileNameLength);
            FileHandling.DeleteFile(inputFilePath);
        }
    }
}
