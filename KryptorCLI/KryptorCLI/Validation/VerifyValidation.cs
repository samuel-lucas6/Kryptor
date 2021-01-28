using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    public static class VerifyValidation
    {
        private static readonly string _filePathError = "Please specify a file to verify.";

        public static bool Verify(char[] encodedPublicKey, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetVerifyErrors(encodedPublicKey, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetVerifyErrors(char[] encodedPublicKey, string[] filePaths)
        {
            if (encodedPublicKey.Length != Constants.PublicKeyLength)
            {
                yield return ValidationMessages.PublicKeyString;
            }
            if (filePaths == null)
            {
                yield return _filePathError;
            }
        }

        public static bool Verify(string publicKeyPath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetVerifyErrors(publicKeyPath, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetVerifyErrors(string publicKeyPath, string[] filePaths)
        {
            if (!publicKeyPath.EndsWith(Constants.PublicKeyExtension) || !File.Exists(publicKeyPath))
            {
                yield return ValidationMessages.PublicKeyFile;
            }
            if (filePaths == null)
            {
                yield return _filePathError;
            }
        }

        public static string GetSignatureFilePath(ref string[] filePaths)
        {
            string signatureFilePath = Array.Find(filePaths, element => element.EndsWith(Constants.SignatureExtension));
            filePaths = filePaths.Where(element => !element.EndsWith(Constants.SignatureExtension)).ToArray();
            // If user didn't specify signature file
            if (string.IsNullOrEmpty(signatureFilePath) && filePaths.Length > 0)
            {
                string possibleSignaturePath = filePaths[0] + Constants.SignatureExtension;
                if (File.Exists(possibleSignaturePath))
                {
                    signatureFilePath = possibleSignaturePath;
                }
            }
            return signatureFilePath;
        }

        public static bool SignatureFile(string signatureFilePath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetSignatureFileError(signatureFilePath, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetSignatureFileError(string signatureFilePath, string[] filePaths)
        {
            if (string.IsNullOrEmpty(signatureFilePath) || filePaths.Length == 0)
            {
                yield return "Please specify a signature file and file to verify.";
            }
            else
            {
                bool? validMagicBytes = FileHandling.IsSignatureFile(signatureFilePath);
                if (validMagicBytes == null)
                {
                    yield return "Unable to access signature file.";
                }
                if (validMagicBytes == false)
                {
                    yield return "Please specify a valid signature file.";
                }
            }
            if (filePaths.Length > 1)
            {
                yield return "Please specify one file to verify.";
            }
        }
    }
}
