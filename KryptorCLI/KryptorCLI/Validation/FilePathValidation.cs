using System;
using System.Collections.Generic;
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
    public static class FilePathValidation
    {
        private static readonly string _fileDoesNotExist = "This file/folder doesn't exist.";
        private static readonly string _fileInaccessible = "Unable to access the file.";

        public static bool FileEncryption(string inputFilePath)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionError(inputFilePath);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileEncryptionError(string inputFilePath)
        {
            if (Directory.Exists(inputFilePath))
            {
                yield break;
            }
            if (!File.Exists(inputFilePath))
            {
                yield return _fileDoesNotExist;
            }
            else
            {
                bool? validMagicBytes = FileHandling.IsKryptorFile(inputFilePath);
                if (validMagicBytes == null)
                {
                    yield return _fileInaccessible;
                }
                else if (FileHandling.HasKryptorExtension(inputFilePath) || validMagicBytes == true)
                {
                    yield return "This file is already encrypted.";
                }
            }
        }

        public static string KeyfilePath(string keyfilePath)
        {
            try
            {
                const string keyfileExtension = ".key";
                if (File.Exists(keyfilePath)) { return keyfilePath; }
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
            IEnumerable<string> errorMessages = GetFileDecryptionError(inputFilePath);
            return DisplayMessage.AnyErrors(errorMessages) || inputFilePath.Contains(Constants.SaltFile);
        }

        private static IEnumerable<string> GetFileDecryptionError(string inputFilePath)
        {
            if (Directory.Exists(inputFilePath))
            {
                yield break;
            }
            if (!File.Exists(inputFilePath))
            {
                yield return _fileDoesNotExist;
            }
            else
            {
                bool? validMagicBytes = FileHandling.IsKryptorFile(inputFilePath);
                if (validMagicBytes == null)
                {
                    yield return _fileInaccessible;
                }
                else if (!FileHandling.HasKryptorExtension(inputFilePath) || validMagicBytes == false)
                {
                    yield return "This file hasn't been encrypted.";
                }
            }
        }

        public static bool GenerateKeyPair(string directoryPath, string keyPairName)
        {
            IEnumerable<string> errorMessages = GetGenerateKeyPairError(directoryPath, keyPairName);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetGenerateKeyPairError(string directoryPath, string keyPairName)
        {
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                yield return "This directory doesn't exist.";
            }
            if (string.IsNullOrEmpty(keyPairName) || keyPairName.Length > 50 || ContainsIllegalCharacters(keyPairName))
            {
                yield return "Please enter a valid key pair name.";
            }
            else if (File.Exists(Path.Combine(directoryPath, keyPairName + Constants.PrivateKeyExtension)) || File.Exists(Path.Combine(directoryPath, keyPairName + Constants.PublicKeyExtension)))
            {
                yield return "This key pair name already exists. Please manually delete your old .public & .private key files if you want to overwrite them.";
            }
        }

        private static bool ContainsIllegalCharacters(string fileName)
        {
            char[] invalidFileNameCharacters = Path.GetInvalidFileNameChars();
            foreach (char character in invalidFileNameCharacters)
            {
                if (fileName.Contains(character)) { return true; }
            }
            return false;
        }

        public static bool RecoverPublicKey(string privateKeyPath)
        {
            IEnumerable<string> errorMessages = GetRecoverPublicKeyError(privateKeyPath);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetRecoverPublicKeyError(string privateKeyPath)
        {
            if (!File.Exists(privateKeyPath) || !privateKeyPath.EndsWith(Constants.PrivateKeyExtension))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
        }
    }
}
