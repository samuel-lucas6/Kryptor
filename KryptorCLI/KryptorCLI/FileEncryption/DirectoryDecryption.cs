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
    public static class DirectoryDecryption
    {
        public static void UsingPassword(string directoryPath, byte[] passwordBytes)
        {
            try
            {
                string[] filePaths = GetFiles(directoryPath);
                string saltFilePath = Path.Combine(directoryPath, Constants.SaltFile);
                byte[] salt = File.ReadAllBytes(saltFilePath);
                if (salt.Length != Constants.SaltLength) { throw new ArgumentException("Invalid salt length.", directoryPath); }
                byte[] keyEncryptionKey = Argon2.DeriveKey(passwordBytes, salt);
                DecryptEachFileWithPassword(filePaths, keyEncryptionKey);
                Finalize(directoryPath, saltFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to decrypt the directory.");
            }
        }

        private static void DecryptEachFileWithPassword(string[] filePaths, byte[] keyEncryptionKey)
        {
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                DecryptInputFile(inputFilePath, keyEncryptionKey);
            }
            Arrays.Zero(keyEncryptionKey);
        }

        private static string[] GetFiles(string directoryPath)
        {
            string[] filePaths = FileHandling.GetAllFiles(directoryPath);
            // -1 for the selected directory
            Globals.TotalCount += filePaths.Length - 1;
            return filePaths;
        }

        private static void DecryptInputFile(string inputFilePath, byte[] keyEncryptionKey)
        {
            try
            {
                string outputFilePath = FileDecryption.GetOutputFilePath(inputFilePath);
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                DecryptFile.Initialize(inputFile, outputFilePath, keyEncryptionKey);
                FileDecryption.DecryptionSuccessful(inputFilePath, outputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
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

        public static void UsingPrivateKey(string directoryPath, byte[] sharedSecret, byte[] recipientPrivateKey)
        {
            try
            {
                string[] filePaths = GetFiles(directoryPath);
                DecryptEachFileWithPrivateKey(filePaths, sharedSecret, recipientPrivateKey);
                Finalize(directoryPath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to decrypt the directory.");
            }
        }

        private static void DecryptEachFileWithPrivateKey(string[] filePaths, byte[] sharedSecret, byte[] recipientPrivateKey)
        {
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(sharedSecret, ephemeralSharedSecret, salt);
                DecryptInputFile(inputFilePath, keyEncryptionKey);
                Arrays.Zero(keyEncryptionKey);
            }
        }

        public static void UsingPrivateKey(string directoryPath, byte[] privateKey)
        {
            try
            {
                string[] filePaths = GetFiles(directoryPath);
                DecryptEachFileWithPrivateKey(filePaths, privateKey);
                Finalize(directoryPath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to decrypt the directory.");
            }
        }

        private static void DecryptEachFileWithPrivateKey(string[] filePaths, byte[] privateKey)
        {
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(privateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(ephemeralSharedSecret, salt);
                DecryptInputFile(inputFilePath, keyEncryptionKey);
                Arrays.Zero(keyEncryptionKey);
            }
        }

        private static void Finalize(string directoryPath, string saltFilePath)
        {
            if (Globals.SuccessfulCount != 0 && Globals.SuccessfulCount == Globals.TotalCount)
            {
                FileHandling.DeleteFile(saltFilePath);
            }
            Finalize(directoryPath);
        }

        private static void Finalize(string directoryPath)
        {
            try
            {
                if (Globals.ObfuscateFileNames)
                {
                    RestoreDirectoryNames.AllDirectories(directoryPath);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to restore the directory names.");
            }
        }
    }
}
