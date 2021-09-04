using System.IO;
using System.Linq;
using System.Collections.Generic;

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
    public static class FileEncryptionValidation
    {
        public static bool FileEncryptionWithPassword(bool usePassword, string keyfilePath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(usePassword, keyfilePath).Concat(GetEncryptionFilePathErrors(filePaths));
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileEncryptionErrors(bool usePassword, string keyfilePath)
        {
            if (!usePassword && string.IsNullOrEmpty(keyfilePath))
            {
                yield return ValidationMessages.PasswordOrKeyfile;
            }
            if (File.Exists(keyfilePath))
            {
                long keyfileLength = FileHandling.GetFileLength(keyfilePath);
                if (keyfileLength < Constants.KeyfileLength)
                {
                    yield return "Please specify a keyfile that is at least 64 bytes in size.";
                }
            }
            else if (Path.EndsInDirectorySeparator(keyfilePath) && !Directory.Exists(keyfilePath))
            {
                yield return "Please specify a valid keyfile directory.";
            }
        }

        private static IEnumerable<string> GetEncryptionFilePathErrors(string[] filePaths)
        {
            if (filePaths == null)
            {
                yield return ValidationMessages.FilePath;
            }
            else
            {
                foreach (string inputFilePath in filePaths)
                {
                    string errorMessage = FilePathValidation.GetFileEncryptionError(inputFilePath);
                    if (!string.IsNullOrEmpty(errorMessage)) { yield return ValidationMessages.GetFilePathError(inputFilePath, errorMessage); }
                }
            }
        }

        public static bool FileEncryptionWithPublicKey(string privateKeyPath, string publicKeyPath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, publicKeyPath).Concat(GetEncryptionFilePathErrors(filePaths));
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath, string publicKeyPath)
        {
            if (string.IsNullOrEmpty(privateKeyPath))
            {
                if (!File.Exists(Constants.DefaultEncryptionPrivateKeyPath))
                {
                    yield return ValidationMessages.PrivateKeyFile;
                }
            }
            else if (!File.Exists(privateKeyPath) || !privateKeyPath.EndsWith(Constants.PrivateKeyExtension))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
            if (string.IsNullOrEmpty(publicKeyPath))
            {
                if (!File.Exists(Constants.DefaultEncryptionPublicKeyPath))
                {
                    yield return ValidationMessages.PublicKeyFile;
                }
            }
            else if (!publicKeyPath.EndsWith(Constants.PublicKeyExtension) || !File.Exists(publicKeyPath))
            {
                yield return ValidationMessages.PublicKeyFile;
            }
        }

        public static bool FileEncryptionWithPublicKey(string privateKeyPath, char[] encodedPublicKey, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, encodedPublicKey).Concat(GetEncryptionFilePathErrors(filePaths));
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath, char[] encodedPublicKey)
        {
            if (string.IsNullOrEmpty(privateKeyPath))
            {
                if (!File.Exists(Constants.DefaultEncryptionPrivateKeyPath))
                {
                    yield return ValidationMessages.PrivateKeyFile;
                }
            }
            else if (!File.Exists(privateKeyPath) || !privateKeyPath.EndsWith(Constants.PrivateKeyExtension))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
            if (encodedPublicKey.Length != Constants.PublicKeyLength)
            {
                yield return ValidationMessages.PublicKeyString;
            }
        }

        public static bool FileEncryptionWithPrivateKey(string privateKeyPath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetEncryptionFilePathErrors(filePaths));
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath)
        {
            if (string.IsNullOrEmpty(privateKeyPath))
            {
                if (!File.Exists(Constants.DefaultEncryptionPrivateKeyPath))
                {
                    yield return ValidationMessages.PrivateKeyFile;
                }
            }
            else if (!File.Exists(privateKeyPath) || !privateKeyPath.EndsWith(Constants.PrivateKeyExtension))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
        }

        public static bool FileDecryptionWithPassword(bool usePassword, string keyfilePath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileDecryptionErrors(usePassword, keyfilePath).Concat(GetDecryptionFilePathErrors(filePaths));
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileDecryptionErrors(bool usePassword, string keyfilePath)
        {
            if (!usePassword && string.IsNullOrEmpty(keyfilePath))
            {
                yield return ValidationMessages.PasswordOrKeyfile;
            }
            if (!string.IsNullOrEmpty(keyfilePath) && !File.Exists(keyfilePath))
            {
                yield return "Please specify a keyfile that exists.";
            }
        }

        private static IEnumerable<string> GetDecryptionFilePathErrors(string[] filePaths)
        {
            if (filePaths == null)
            {
                yield return ValidationMessages.FilePath;
            }
            else
            {
                foreach (string inputFilePath in filePaths)
                {
                    string errorMessage = FilePathValidation.GetFileDecryptionError(inputFilePath);
                    if (!string.IsNullOrEmpty(errorMessage)) { yield return ValidationMessages.GetFilePathError(inputFilePath, errorMessage); }
                }
            }
        }

        public static bool FileDecryptionWithPublicKey(string privateKeyPath, string publicKeyPath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, publicKeyPath).Concat(GetDecryptionFilePathErrors(filePaths));
            return DisplayMessage.AnyErrors(errorMessages);
        }

        public static bool FileDecryptionWithPublicKey(string privateKeyPath, char[] encodedPublicKey, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, encodedPublicKey).Concat(GetDecryptionFilePathErrors(filePaths));
            return DisplayMessage.AnyErrors(errorMessages);
        }

        public static bool FileDecryptionWithPrivateKey(string privateKeyPath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetDecryptionFilePathErrors(filePaths));
            return DisplayMessage.AnyErrors(errorMessages);
        }
    }
}
