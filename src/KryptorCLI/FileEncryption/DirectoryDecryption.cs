using System;
using System.IO;
using System.Security.Cryptography;

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
                if (!File.Exists(saltFilePath)) { throw new FileNotFoundException("No salt file found. Unable to decrypt the directory. Please decrypt these files individually."); }
                byte[] salt = File.ReadAllBytes(saltFilePath);
                if (salt.Length != Constants.SaltLength) { throw new ArgumentException("Invalid salt length."); }
                byte[] keyEncryptionKey = Argon2.DeriveKey(passwordBytes, salt);
                DecryptEachFileWithPassword(filePaths, keyEncryptionKey);
                Finalize(directoryPath, saltFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                if (ex is ArgumentException || ex is FileNotFoundException)
                {
                    DisplayMessage.FilePathError(directoryPath, ex.Message);
                    return;
                }
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
                try
                {
                    using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                    DecryptInputFile(inputFile, keyEncryptionKey);
                }
                catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
                {
                    FileException(inputFilePath, ex);
                }
            }
            CryptographicOperations.ZeroMemory(keyEncryptionKey);
        }

        private static string[] GetFiles(string directoryPath)
        {
            string[] filePaths = FileHandling.GetAllFiles(directoryPath);
            // -1 for the selected directory
            Globals.TotalCount += filePaths.Length - 1;
            return filePaths;
        }

        private static void DecryptInputFile(FileStream inputFile, byte[] keyEncryptionKey)
        {
            string inputFilePath = inputFile.Name;
            string outputFilePath = FileDecryption.GetOutputFilePath(inputFilePath);
            DecryptFile.Initialize(inputFile, outputFilePath, keyEncryptionKey);
            FileDecryption.DecryptionSuccessful(inputFilePath, outputFilePath);
        }

        public static void UsingPublicKey(string directoryPath, byte[] sharedSecret, byte[] recipientPrivateKey)
        {
            try
            {
                string[] filePaths = GetFiles(directoryPath);
                DecryptEachFileWithPublicKey(filePaths, sharedSecret, recipientPrivateKey);
                Finalize(directoryPath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to decrypt the directory.");
            }
        }

        private static void DecryptEachFileWithPublicKey(string[] filePaths, byte[] sharedSecret, byte[] recipientPrivateKey)
        {
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                try
                {
                    using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                    byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                    byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, ephemeralPublicKey);
                    byte[] salt = FileHeaders.ReadSalt(inputFile);
                    byte[] keyEncryptionKey = Generate.KeyEncryptionKey(sharedSecret, ephemeralSharedSecret, salt);
                    DecryptInputFile(inputFile, keyEncryptionKey);
                    CryptographicOperations.ZeroMemory(keyEncryptionKey);
                }
                catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
                {
                    FileException(inputFilePath, ex);
                }
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
                try
                {
                    using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                    byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                    byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(privateKey, ephemeralPublicKey);
                    byte[] salt = FileHeaders.ReadSalt(inputFile);
                    byte[] keyEncryptionKey = Generate.KeyEncryptionKey(ephemeralSharedSecret, salt);
                    DecryptInputFile(inputFile, keyEncryptionKey);
                    CryptographicOperations.ZeroMemory(keyEncryptionKey);
                }
                catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
                {
                    FileException(inputFilePath, ex);
                }
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
                RestoreDirectoryNames.AllDirectories(directoryPath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to restore the directory names.");
            }
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
