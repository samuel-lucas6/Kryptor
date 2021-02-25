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
    public static class DirectoryEncryption
    {
        public static void UsingPassword(string directoryPath, byte[] passwordBytes)
        {
            try
            {
                string[] filePaths = GetFiles(ref directoryPath);
                // Generate one salt for the entire directory
                byte[] salt = Generate.Salt();
                CreateSaltFile(directoryPath, salt);
                // Perform password hashing once
                byte[] keyEncryptionKey = Argon2.DeriveKey(passwordBytes, salt);
                EncryptEachFileWithPassword(filePaths, salt, keyEncryptionKey);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to encrypt the directory.");
            }
        }

        private static string[] GetFiles(ref string directoryPath)
        {
            try
            {
                if (Globals.ObfuscateFileNames)
                {
                    directoryPath = ObfuscateDirectoryNames.AllDirectories(directoryPath);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to obfuscate the directory names.");
            }
            string[] filePaths = FileHandling.GetAllFiles(directoryPath);
            // -1 for the selected directory
            Globals.TotalCount += filePaths.Length - 1;
            return filePaths;
        }

        private static void CreateSaltFile(string directoryPath, byte[] salt)
        {
            string saltFilePath = Path.Combine(directoryPath, Constants.SaltFile);
            File.WriteAllBytes(saltFilePath, salt);
            FileHandling.SetFileAttributesReadOnly(saltFilePath);
        }

        private static void EncryptEachFileWithPassword(string[] filePaths, byte[] salt, byte[] keyEncryptionKey)
        {
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileEncryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                // Fill unused header with random public key
                byte[] ephemeralPublicKey = Generate.EphemeralPublicKeyHeader();
                string outputFilePath = FileEncryption.GetOutputFilePath(inputFilePath);
                EncryptInputFile(inputFilePath, outputFilePath, ephemeralPublicKey, salt, keyEncryptionKey);
            }
            CryptographicOperations.ZeroMemory(keyEncryptionKey);
        }

        private static void EncryptInputFile(string inputFilePath, string outputFilePath, byte[] ephemeralPublicKey, byte[] salt, byte[] keyEncryptionKey)
        {
            try
            {
                EncryptFile.Initialize(inputFilePath, outputFilePath, ephemeralPublicKey, salt, keyEncryptionKey);
                FileEncryption.EncryptionSuccessful(inputFilePath, outputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to encrypt the file.");
            }
        }

        public static void UsingPublicKey(string directoryPath, byte[] sharedSecret, byte[] recipientPublicKey)
        {
            try
            {
                string[] filePaths = GetFiles(ref directoryPath);
                EncryptEachFileWithPublicKey(filePaths, sharedSecret, recipientPublicKey);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to encrypt the directory.");
            }
        }

        private static void EncryptEachFileWithPublicKey(string[] filePaths, byte[] sharedSecret, byte[] recipientPublicKey)
        {
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileEncryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                // Derive a unique KEK per file
                byte[] ephemeralSharedSecret = KeyExchange.GetEphemeralSharedSecret(recipientPublicKey, out byte[] ephemeralPublicKey);
                byte[] salt = Generate.Salt();
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(sharedSecret, ephemeralSharedSecret, salt);
                string outputFilePath = FileEncryption.GetOutputFilePath(inputFilePath);
                EncryptInputFile(inputFilePath, outputFilePath, ephemeralPublicKey, salt, keyEncryptionKey);
                CryptographicOperations.ZeroMemory(keyEncryptionKey);
            }
        }

        public static void UsingPrivateKey(string directoryPath, byte[] privateKey)
        {
            try
            {
                string[] filePaths = GetFiles(ref directoryPath);
                EncryptEachFileWithPrivateKey(filePaths, privateKey);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to encrypt the directory.");
            }
        }

        private static void EncryptEachFileWithPrivateKey(string[] filePaths, byte[] privateKey)
        {
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileEncryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                // Derive a unique KEK per file
                byte[] ephemeralSharedSecret = KeyExchange.GetPrivateKeySharedSecret(privateKey, out byte[] ephemeralPublicKey);
                byte[] salt = Generate.Salt();
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(ephemeralSharedSecret, salt);
                string outputFilePath = FileEncryption.GetOutputFilePath(inputFilePath);
                EncryptInputFile(inputFilePath, outputFilePath, ephemeralPublicKey, salt, keyEncryptionKey);
                CryptographicOperations.ZeroMemory(keyEncryptionKey);
            }
        }
    }
}
