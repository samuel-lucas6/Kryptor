using System;
using System.IO;

/*
    Kryptor: A simple, modern, and secure encryption tool.
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
    public static class FileDecryption
    {
        public static void DecryptEachFileWithPassword(string[] filePaths, byte[] passwordBytes)
        {
            Globals.TotalCount = filePaths.Length;
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                UsingPassword(inputFilePath, passwordBytes);
            }
            Arrays.Zero(passwordBytes);
            DisplayMessage.SuccessfullyDecrypted();
        }

        private static void UsingPassword(string inputFilePath, byte[] passwordBytes)
        {
            try
            {
                bool fileIsDirectory = FileHandling.IsDirectory(inputFilePath);
                if (fileIsDirectory)
                {
                    DirectoryDecryption.UsingPassword(inputFilePath, passwordBytes);
                    return;
                }
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                byte[] keyEncryptionKey = Argon2.DeriveKey(passwordBytes, salt);
                string outputFilePath = GetOutputFilePath(inputFilePath);
                DecryptFile.Initialize(inputFile, outputFilePath, keyEncryptionKey);
                Arrays.Zero(keyEncryptionKey);
                DecryptionSuccessful(inputFilePath, outputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                FileException(inputFilePath, ex);
            }
        }

        public static void DecryptEachFileWithPublicKey(string[] filePaths, byte[] recipientPrivateKey, byte[] senderPublicKey)
        {
            Globals.TotalCount = filePaths.Length;
            recipientPrivateKey = PrivateKey.Decrypt(recipientPrivateKey);
            if (recipientPrivateKey == null) { return; }
            byte[] sharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, senderPublicKey);
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                UsingPublicKey(inputFilePath, sharedSecret, recipientPrivateKey);
            }
            Arrays.Zero(recipientPrivateKey);
            Arrays.Zero(sharedSecret);
            DisplayMessage.SuccessfullyDecrypted();
        }

        private static void UsingPublicKey(string inputFilePath, byte[] sharedSecret, byte[] recipientPrivateKey)
        {
            try
            {
                bool fileIsDirectory = FileHandling.IsDirectory(inputFilePath);
                if (fileIsDirectory)
                {
                    DirectoryDecryption.UsingPublicKey(inputFilePath, sharedSecret, recipientPrivateKey);
                    return;
                }
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(sharedSecret, ephemeralSharedSecret, salt);
                string outputFilePath = GetOutputFilePath(inputFilePath);
                DecryptFile.Initialize(inputFile, outputFilePath, keyEncryptionKey);
                Arrays.Zero(keyEncryptionKey);
                DecryptionSuccessful(inputFilePath, outputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                FileException(inputFilePath, ex);
            }
        }

        public static void DecryptEachFileWithPrivateKey(string[] filePaths, byte[] privateKey)
        {
            Globals.TotalCount = filePaths.Length;
            privateKey = PrivateKey.Decrypt(privateKey);
            if (privateKey == null) { return; }
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                UsingPrivateKey(inputFilePath, privateKey);
            }
            Arrays.Zero(privateKey);
            DisplayMessage.SuccessfullyDecrypted();
        }

        private static void UsingPrivateKey(string inputFilePath, byte[] privateKey)
        {
            try
            {
                bool fileIsDirectory = FileHandling.IsDirectory(inputFilePath);
                if (fileIsDirectory)
                {
                    DirectoryDecryption.UsingPrivateKey(inputFilePath, privateKey);
                    return;
                }
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(privateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(ephemeralSharedSecret, salt);
                string outputFilePath = GetOutputFilePath(inputFilePath);
                DecryptFile.Initialize(inputFile, outputFilePath, keyEncryptionKey);
                Arrays.Zero(keyEncryptionKey);
                DecryptionSuccessful(inputFilePath, outputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                FileException(inputFilePath, ex);
            }
        }

        public static string GetOutputFilePath(string inputFilePath)
        {
            string outputFilePath = inputFilePath.Replace(Constants.EncryptedExtension, string.Empty);
            return FileHandling.GetUniqueFilePath(outputFilePath);
        }

        public static void DecryptionSuccessful(string inputFilePath, string outputFilePath)
        {
            if (!Globals.ObfuscateFileNames)
            {
                DisplayMessage.FileEncryptionResult(inputFilePath, outputFilePath);
            }
            Globals.SuccessfulCount += 1;
        }

        private static void FileException(string inputFilePath, Exception ex)
        {
            Logging.LogException(ex.ToString(), Logging.Severity.Error);
            if (ex is ArgumentException)
            {
                DisplayMessage.FilePathMessage(inputFilePath, ex.Message);
                return;
            }
            DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to decrypt the file.");
        }
    }
}
