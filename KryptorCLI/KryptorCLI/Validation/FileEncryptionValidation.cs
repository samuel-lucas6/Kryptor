using System.Collections.Generic;
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
    public static class FileEncryptionValidation
    {
        public static bool FileEncryptionWithPassword(char[] password, string keyfilePath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(password, keyfilePath, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileEncryptionErrors(char[] password, string keyfilePath, string[] filePaths)
        {
            if (password.Length == 0 && string.IsNullOrEmpty(keyfilePath))
            {
                yield return "Please specify whether to use a password and/or keyfile.";
            }
            if (filePaths == null)
            {
                yield return ValidationMessages.FilePath;
            }
        }

        public static bool FileEncryptionWithPublicKey(string privateKeyPath, string publicKeyPath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, publicKeyPath, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath, string publicKeyPath, string[] filePaths)
        {
            if (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension) || !File.Exists(privateKeyPath))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
            if (!publicKeyPath.EndsWith(Constants.PublicKeyExtension) || !File.Exists(publicKeyPath))
            {
                yield return ValidationMessages.PublicKeyFile;
            }
            if (filePaths == null)
            {
                yield return ValidationMessages.FilePath;
            }
        }

        public static bool FileEncryptionWithPublicKey(string privateKeyPath, char[] encodedPublicKey, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, encodedPublicKey, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath, char[] encodedPublicKey, string[] filePaths)
        {
            if (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension) || !File.Exists(privateKeyPath))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
            if (encodedPublicKey.Length != Constants.PublicKeyLength)
            {
                yield return ValidationMessages.PublicKeyString;
            }
            if (filePaths == null)
            {
                yield return ValidationMessages.FilePath;
            }
        }

        public static bool FileEncryptionWithPrivateKey(string privateKeyPath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath, string[] filePaths)
        {
            if (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension) || !File.Exists(privateKeyPath))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
            if (filePaths == null)
            {
                yield return ValidationMessages.FilePath;
            }
        }
    }
}
