using System;
using System.Text;
using System.Security.Cryptography;
using Sodium;

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
    public static class FileEncryption
    {
        public static void EncryptEachFileWithPassword(string[] filePaths, byte[] passwordBytes)
        {
            Globals.TotalCount = filePaths.Length;
            foreach (string inputFilePath in filePaths)
            {
                UsingPassword(inputFilePath, passwordBytes);
            }
            CryptographicOperations.ZeroMemory(passwordBytes);
            DisplayMessage.SuccessfullyEncrypted();
        }

        private static void UsingPassword(string inputFilePath, byte[] passwordBytes)
        {
            try
            {
                bool fileIsDirectory = FileHandling.IsDirectory(inputFilePath);
                if (fileIsDirectory)
                {
                    DirectoryEncryption.UsingPassword(inputFilePath, passwordBytes);
                    return;
                }
                // Derive a unique KEK per file
                byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                byte[] keyEncryptionKey = Argon2.DeriveKey(passwordBytes, salt);
                // Fill unused header with random public key
                using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
                string outputFilePath = GetOutputFilePath(inputFilePath);
                EncryptFile.Initialize(inputFilePath, outputFilePath, ephemeralKeyPair.PublicKey, salt, keyEncryptionKey);
                CryptographicOperations.ZeroMemory(keyEncryptionKey);
                EncryptionSuccessful(inputFilePath, outputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to encrypt the file.");
            }
        }

        public static void EncryptEachFileWithPublicKey(string[] filePaths, byte[] senderPrivateKey, byte[] recipientPublicKey)
        {
            Globals.TotalCount = filePaths.Length;
            senderPrivateKey = PrivateKey.Decrypt(senderPrivateKey);
            if (senderPrivateKey == null) { return; }
            byte[] sharedSecret = KeyExchange.GetSharedSecret(senderPrivateKey, recipientPublicKey);
            CryptographicOperations.ZeroMemory(senderPrivateKey);
            foreach (string inputFilePath in filePaths)
            {
                UsingPublicKey(inputFilePath, sharedSecret, recipientPublicKey);
            }
            CryptographicOperations.ZeroMemory(sharedSecret);
            DisplayMessage.SuccessfullyEncrypted();
        }

        private static void UsingPublicKey(string inputFilePath, byte[] sharedSecret, byte[] recipientPublicKey)
        {
            try
            {
                bool fileIsDirectory = FileHandling.IsDirectory(inputFilePath);
                if (fileIsDirectory)
                {
                    DirectoryEncryption.UsingPublicKey(inputFilePath, sharedSecret, recipientPublicKey);
                    return;
                }
                // Derive a unique KEK per file
                byte[] ephemeralSharedSecret = KeyExchange.GetEphemeralSharedSecret(recipientPublicKey, out byte[] ephemeralPublicKey);
                byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(sharedSecret, ephemeralSharedSecret, salt);
                string outputFilePath = GetOutputFilePath(inputFilePath);
                EncryptFile.Initialize(inputFilePath, outputFilePath, ephemeralPublicKey, salt, keyEncryptionKey);
                CryptographicOperations.ZeroMemory(keyEncryptionKey);
                EncryptionSuccessful(inputFilePath, outputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to encrypt the file.");
            }
        }

        public static void EncryptEachFileWithPrivateKey(string[] filePaths, byte[] privateKey)
        {
            Globals.TotalCount = filePaths.Length;
            privateKey = PrivateKey.Decrypt(privateKey);
            if (privateKey == null) { return; }
            foreach (string inputFilePath in filePaths)
            {
                UsingPrivateKey(inputFilePath, privateKey);
            }
            CryptographicOperations.ZeroMemory(privateKey);
            DisplayMessage.SuccessfullyEncrypted();
        }

        private static void UsingPrivateKey(string inputFilePath, byte[] privateKey)
        {
            try
            {
                bool fileIsDirectory = FileHandling.IsDirectory(inputFilePath);
                if (fileIsDirectory)
                {
                    DirectoryEncryption.UsingPrivateKey(inputFilePath, privateKey);
                    return;
                }
                // Derive a unique KEK per file
                byte[] ephemeralSharedSecret = KeyExchange.GetPrivateKeySharedSecret(privateKey, out byte[] ephemeralPublicKey);
                byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(ephemeralSharedSecret, salt);
                string outputFilePath = GetOutputFilePath(inputFilePath);
                EncryptFile.Initialize(inputFilePath, outputFilePath, ephemeralPublicKey, salt, keyEncryptionKey);
                CryptographicOperations.ZeroMemory(keyEncryptionKey);
                EncryptionSuccessful(inputFilePath, outputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to encrypt the file.");
            }
        }

        public static string GetOutputFilePath(string inputFilePath)
        {
            try
            {
                if (Globals.ObfuscateFileNames)
                {
                    ObfuscateFileName.AppendFileName(inputFilePath);
                    inputFilePath = ObfuscateFileName.ReplaceFilePath(inputFilePath);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex) || ex is EncoderFallbackException)
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to store file name.");
            }
            string outputFilePath = inputFilePath + Constants.EncryptedExtension;
            return FileHandling.GetUniqueFilePath(outputFilePath);
        }

        public static void EncryptionSuccessful(string inputFilePath, string outputFilePath)
        {
            DisplayMessage.FileEncryptionResult(inputFilePath, outputFilePath);
            Globals.SuccessfulCount += 1;
        }
    }
}
