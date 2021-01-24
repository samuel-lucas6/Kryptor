using System;
using Sodium;
using System.IO;

/*
    Kryptor: Modern and secure file encryption.
    Copyright(C) 2020 Samuel Lucas

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
    public static class EncryptFile
    {
        public static void Initialize(string inputFilePath, string outputFilePath, byte[] ephemeralPublicKey, byte[] salt, byte[] keyEncryptionKey)
        {
            byte[] dataEncryptionKey = Generate.RandomDataEncryptionKey();
            try
            {
                byte[] headerNonce = Generate.RandomNonce();
                byte[] fileNonce = Generate.RandomNonce();
                byte[] encryptedHeader = EncryptFileHeader(inputFilePath, fileNonce, dataEncryptionKey, headerNonce, keyEncryptionKey);
                using var outputFile = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan);
                FileHeaders.WriteHeaders(outputFile, ephemeralPublicKey, salt, headerNonce, encryptedHeader);
                byte[] additionalData = ChunkHandling.GetPreviousPoly1305Tag(encryptedHeader);
                File.SetAttributes(inputFilePath, FileAttributes.Normal);
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan);
                Encrypt(inputFile, outputFile, fileNonce, dataEncryptionKey, additionalData);
                Finalize(inputFilePath, outputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                FileHandling.DeleteFile(outputFilePath);
                Utilities.ZeroArray(dataEncryptionKey);
                throw;
            }
        }

        private static byte[] EncryptFileHeader(string inputFilePath, byte[] fileNonce, byte[] dataEncryptionKey, byte[] headerNonce, byte[] keyEncryptionKey)
        {
            byte[] keyCommitmentBlock = ChunkHandling.GetKeyCommitmentBlock();
            long fileLength = FileHandling.GetFileLength(inputFilePath);
            byte[] lastChunkLength = BitConverter.GetBytes(Convert.ToInt32(fileLength % Constants.FileChunkSize));
            byte[] fileNameLength = FileHeaders.GetFileNameLength(inputFilePath);
            byte[] fileHeader = Utilities.ConcatArrays(keyCommitmentBlock, lastChunkLength, fileNameLength, fileNonce, dataEncryptionKey);
            byte[] additionalData = HeaderEncryption.ComputeAdditionalData(fileLength);
            return HeaderEncryption.Encrypt(fileHeader, headerNonce, keyEncryptionKey, additionalData);
        }

        private static void Encrypt(FileStream inputFile, FileStream outputFile, byte[] nonce, byte[] dataEncryptionKey, byte[] additionalData)
        {
            const int offset = 0;
            byte[] plaintext = new byte[Constants.FileChunkSize];
            while (inputFile.Read(plaintext, offset, plaintext.Length) > 0)
            {
                byte[] plaintextChunk = ChunkHandling.PrependKeyCommitmentBlock(plaintext);
                byte[] ciphertextChunk = SecretAeadXChaCha20Poly1305.Encrypt(plaintextChunk, nonce, dataEncryptionKey, additionalData);
                nonce = Sodium.Utilities.Increment(nonce);
                additionalData = ChunkHandling.GetPreviousPoly1305Tag(ciphertextChunk);
                outputFile.Write(ciphertextChunk, offset, ciphertextChunk.Length);
            }
            Utilities.ZeroArray(dataEncryptionKey);
        }

        private static void Finalize(string inputFilePath, string outputFilePath)
        {
            if (Globals.Overwrite)
            {
                FileHandling.OverwriteFile(inputFilePath, outputFilePath);
            }
            else if (Globals.ObfuscateFileNames)
            {
                RestoreFileName.RemoveAppendedFileName(inputFilePath);
            }
            FileHandling.MakeFileReadOnly(outputFilePath);
        }
    }
}
