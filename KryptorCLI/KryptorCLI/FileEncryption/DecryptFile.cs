using System;
using Sodium;
using System.IO;
using System.Security.Cryptography;

/*
    Kryptor: Free and open source file encryption.
    Copyright(C) 2020-2021 Samuel Lucas

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
        public static void Initialize(string inputFilePath, string outputFilePath, byte[] keyEncryptionKey)
        {
            byte[] dataEncryptionKey = new byte[Constants.EncryptionKeyLength];
            try
            {
                byte[] encryptedHeader = FileHeaders.ReadEncryptedHeader(inputFilePath);
                byte[] nonce = FileHeaders.ReadNonce(inputFilePath);
                byte[] header = DecryptFileHeader(inputFilePath, encryptedHeader, nonce, keyEncryptionKey);
                if (header == null) { throw new ArgumentException("Incorrect password/keyfile or this file has been tampered with."); }
                ChunkHandling.ValidateKeyCommitmentBlock(header);
                int lastChunkLength = FileHeaders.GetLastChunkLength(header);
                int fileNameLength = FileHeaders.GetFileNameLength(header);
                dataEncryptionKey = FileHeaders.GetDataEncryptionKey(header);
                Utilities.ZeroArray(header);
                using (var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan))
                using (var outputFile = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan))
                {
                    nonce = Sodium.Utilities.Increment(nonce);
                    byte[] additionalData = ChunkHandling.GetPreviousPoly1305Tag(encryptedHeader);
                    Decrypt(inputFile, outputFile, nonce, dataEncryptionKey, additionalData, lastChunkLength);
                }
                Finalize(inputFilePath, outputFilePath, fileNameLength);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                Utilities.ZeroArray(dataEncryptionKey);
                throw;
            }
        }

        private static byte[] DecryptFileHeader(string inputFilePath, byte[] encryptedHeader, byte[] nonce, byte[] keyEncryptionKey)
        {
            byte[] additionalData = HeaderEncryption.GetAdditionalData(inputFilePath);
            return HeaderEncryption.Decrypt(encryptedHeader, nonce, keyEncryptionKey, additionalData);
        }

        private static void Decrypt(FileStream inputFile, FileStream outputFile, byte[] nonce, byte[] dataEncryptionKey, byte[] additionalData, int lastChunkLength)
        {
            int headersLength = FileHeaders.GetHeadersLength();
            inputFile.Seek(headersLength, SeekOrigin.Begin);
            const int offset = 0;
            byte[] ciphertextChunk = new byte[Constants.TotalChunkLength];
            while (inputFile.Read(ciphertextChunk, offset, ciphertextChunk.Length) > 0)
            {
                byte[] plaintextChunk = SecretAeadXChaCha20Poly1305.Decrypt(ciphertextChunk, nonce, dataEncryptionKey, additionalData);
                ChunkHandling.ValidateKeyCommitmentBlock(plaintextChunk);
                nonce = Sodium.Utilities.Increment(nonce);
                additionalData = ChunkHandling.GetPreviousPoly1305Tag(ciphertextChunk);
                plaintextChunk = ChunkHandling.RemoveKeyCommitmentBlock(plaintextChunk);
                outputFile.Write(plaintextChunk, offset, plaintextChunk.Length);
            }
            outputFile.SetLength((outputFile.Length - Constants.FileChunkSize) + lastChunkLength);
            Utilities.ZeroArray(dataEncryptionKey);
        }

        private static void Finalize(string inputFilePath, string outputFilePath, int fileNameLength)
        {
            RestoreFileName.RenameFile(outputFilePath, fileNameLength);
            FileHandling.DeleteFile(inputFilePath);
        }
    }
}
