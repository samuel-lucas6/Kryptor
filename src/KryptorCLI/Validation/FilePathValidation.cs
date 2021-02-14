using System;
using System.Collections.Generic;
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
    public static class FilePathValidation
    {
        private static readonly string _fileDoesNotExist = "This file/folder doesn't exist.";
        private static readonly string _fileInaccessible = "Unable to access the file.";

        public static bool FileEncryption(string inputFilePath)
        {
            string errorMessage = GetFileEncryptionError(inputFilePath);
            if (string.IsNullOrEmpty(errorMessage)) { return true; }
            DisplayMessage.Error(errorMessage);
            return false;
        }

        private static string GetFileEncryptionError(string inputFilePath)
        {
            if (Directory.Exists(inputFilePath)) { return null; }
            if (!File.Exists(inputFilePath)) { return _fileDoesNotExist; }
            bool? validMagicBytes = FileHandling.IsKryptorFile(inputFilePath);
            if (validMagicBytes == null) { return _fileInaccessible; }
            if (FileHandling.HasKryptorExtension(inputFilePath) || validMagicBytes == true)
            {
                return "This file is already encrypted.";
            }
            return null;
        }

        public static string KeyfilePath(string keyfilePath)
        {
            try
            {
                const string keyfileExtension = ".key";
                if (string.IsNullOrEmpty(keyfilePath) || File.Exists(keyfilePath)) { return keyfilePath; }
                // Generate a random keyfile
                if (Directory.Exists(keyfilePath))
                {
                    string randomFileName = ObfuscateFileName.GetRandomFileName() + keyfileExtension;
                    keyfilePath = Path.Combine(keyfilePath, randomFileName);
                }
                // Append keyfile extension if missing
                if (!keyfilePath.EndsWith(keyfileExtension, StringComparison.InvariantCulture))
                {
                    keyfilePath += keyfileExtension;
                }
                Keyfiles.GenerateKeyfile(keyfilePath);
                return keyfilePath;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.FilePathException(keyfilePath, ex.GetType().Name, "Unable to randomly generate keyfile.");
                return null;
            }
        }

        public static bool FileDecryption(string inputFilePath)
        {
            if (inputFilePath.Contains(Constants.SaltFile)) { return false; }
            string errorMessage = GetFileDecryptionError(inputFilePath);
            if (string.IsNullOrEmpty(errorMessage)) { return true; }
            DisplayMessage.Error(errorMessage);
            return false;
        }

        private static string GetFileDecryptionError(string inputFilePath)
        {
            if (Directory.Exists(inputFilePath)) { return null; }
            if (!File.Exists(inputFilePath)) { return _fileDoesNotExist; }
            bool? validMagicBytes = FileHandling.IsKryptorFile(inputFilePath);
            if (validMagicBytes == null) { return _fileInaccessible; }
            if (!FileHandling.HasKryptorExtension(inputFilePath) || validMagicBytes == false)
            {
                return "This file hasn't been encrypted.";
            }
            return null;
        }

        public static bool GenerateKeyPair(string directoryPath, int keyPairType)
        {
            IEnumerable<string> errorMessages = GetGenerateKeyPairError(directoryPath, keyPairType);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetGenerateKeyPairError(string directoryPath, int keyPairType)
        {
            if (keyPairType < 1 || keyPairType > 2)
            {
                yield return "Please enter a valid number.";
            }
            bool defaultKeyDirectory = string.Equals(directoryPath, Constants.DefaultKeyDirectory);
            if (!defaultKeyDirectory && !Directory.Exists(directoryPath))
            {
                yield return "This directory doesn't exist.";
            }
            else if (defaultKeyDirectory && !Globals.Overwrite && keyPairType == 1)
            {
                if (File.Exists(Constants.DefaultEncryptionPublicKeyPath) || File.Exists(Constants.DefaultEncryptionPrivateKeyPath))
                {
                    yield return "An encryption key pair already exists. Please use -o|--overwrite if you want to overwrite your key pair.";
                }
            }
            else if (defaultKeyDirectory && !Globals.Overwrite && keyPairType == 2)
            {
                if (File.Exists(Constants.DefaultSigningPublicKeyPath) || File.Exists(Constants.DefaultSigningPrivateKeyPath))
                {   
                    yield return "A signing key pair already exists. Please use -o|--overwrite if you want to overwrite your key pair.";
                }
            }
            else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 1)
            {
                string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PublicKeyExtension);
                string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PrivateKeyExtension);
                if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath))
                {
                    yield return "An encryption key pair already exists in the specified directory. Please use -o|--overwrite if you want to overwrite your key pair.";
                }
            }
            else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 2)
            {
                string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PublicKeyExtension);
                string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PrivateKeyExtension);
                if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath))
                {
                    yield return "A signing key pair already exists in the specified directory. Please use -o|--overwrite if you want to overwrite your key pair.";
                }
            }
        }

        public static bool RecoverPublicKey(string privateKeyPath)
        {
            IEnumerable<string> errorMessages = GetRecoverPublicKeyError(privateKeyPath);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetRecoverPublicKeyError(string privateKeyPath)
        {
            if (privateKeyPath == null)
            {
                yield return "Please specify the private key using [-x=filepath].";
            }
            else if (!File.Exists(privateKeyPath) || !privateKeyPath.EndsWith(Constants.PrivateKeyExtension))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
        }
    }
}
