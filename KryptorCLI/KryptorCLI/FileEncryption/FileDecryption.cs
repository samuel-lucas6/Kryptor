using System;
using System.IO;

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
            Utilities.ZeroArray(passwordBytes);
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
                byte[] salt = FileHeaders.ReadSalt(inputFilePath);
                byte[] keyEncryptionKey = Argon2.DeriveKey(passwordBytes, salt);
                string outputFilePath = GetOutputFilePath(inputFilePath);
                DecryptFile.Initialize(inputFilePath, outputFilePath, keyEncryptionKey);
                Utilities.ZeroArray(keyEncryptionKey);
                DecryptionSuccessful(inputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                if (ex is ArgumentException || ex is ArgumentOutOfRangeException)
                {
                    DisplayMessage.FilePathMessage(inputFilePath, ex.Message);
                    return;
                }
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to decrypt the file.");
            }
        }

        public static void DecryptEachFileWithPrivateKey(string[] filePaths, byte[] recipientPrivateKey, byte[] senderPublicKey)
        {
            Globals.TotalCount = filePaths.Length;
            recipientPrivateKey = PrivateKey.Decrypt(recipientPrivateKey);
            if (recipientPrivateKey == null) { return; }
            byte[] sharedSecret = KeyExchange.GetLongTermSharedSecret(recipientPrivateKey, senderPublicKey);
            Utilities.ZeroArray(recipientPrivateKey);
            foreach (string inputFilePath in filePaths)
            {
                bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
                if (!validFilePath)
                {
                    --Globals.TotalCount;
                    continue;
                }
                UsingPrivateKey(inputFilePath, sharedSecret, recipientPrivateKey);
            }
            Utilities.ZeroArray(sharedSecret);
            DisplayMessage.SuccessfullyDecrypted();
        }

        private static void UsingPrivateKey(string inputFilePath, byte[] sharedSecret, byte[] recipientPrivateKey)
        {
            try
            {
                bool fileIsDirectory = FileHandling.IsDirectory(inputFilePath);
                if (fileIsDirectory)
                {
                    DirectoryDecryption.UsingPrivateKey(inputFilePath, sharedSecret, recipientPrivateKey);
                    return;
                }
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFilePath);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFilePath);
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(sharedSecret, ephemeralSharedSecret, salt);
                string outputFilePath = GetOutputFilePath(inputFilePath);
                DecryptFile.Initialize(inputFilePath, outputFilePath, keyEncryptionKey);
                Utilities.ZeroArray(keyEncryptionKey);
                DecryptionSuccessful(inputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                if (ex is ArgumentException || ex is ArgumentOutOfRangeException)
                {
                    DisplayMessage.FilePathMessage(inputFilePath, ex.Message);
                    return;
                }
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to decrypt the file.");
            }
        }

        public static void DecryptEachFileWithPrivateKey(string[] filePaths, byte[] privateKey)
        {
            Globals.TotalCount = filePaths.Length;
            privateKey = PrivateKey.Decrypt(privateKey);
            if (privateKey == null) { return; }
            privateKey = KeyExchange.ConvertPrivateKeyToCurve25519(privateKey);
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
            Utilities.ZeroArray(privateKey);
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
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFilePath);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(privateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFilePath);
                byte[] keyEncryptionKey = Generate.KeyEncryptionKey(ephemeralSharedSecret, salt);
                string outputFilePath = GetOutputFilePath(inputFilePath);
                DecryptFile.Initialize(inputFilePath, outputFilePath, keyEncryptionKey);
                Utilities.ZeroArray(keyEncryptionKey);
                DecryptionSuccessful(inputFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                if (ex is ArgumentException || ex is ArgumentOutOfRangeException)
                {
                    DisplayMessage.FilePathMessage(inputFilePath, ex.Message);
                    return;
                }
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to decrypt the file.");
            }
        }

        public static string GetOutputFilePath(string inputFilePath)
        {
            return inputFilePath.Replace(Constants.EncryptedExtension, string.Empty);
        }

        public static void DecryptionSuccessful(string inputFilePath)
        {
            DisplayMessage.FileEncryptionResult(inputFilePath);
            Globals.SuccessfulCount += 1;
        }
    }
}
