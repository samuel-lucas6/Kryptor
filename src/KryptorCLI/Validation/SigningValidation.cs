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
    public static class SigningValidation
    {
        private static readonly string _noFileToVerifyError = "Please specify a file to verify.";
        private static readonly string _invalidSignatureFile = "Please specify a valid signature file.";
        private static readonly string _singleFileToVerifyError = "Please specify a single file to verify.";

        public static bool Sign(string privateKeyPath, string comment, string signatureFilePath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetSignErrors(privateKeyPath, comment, signatureFilePath, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetSignErrors(string privateKeyPath, string comment, string signatureFilePath, string[] filePaths)
        {
            if (string.IsNullOrEmpty(privateKeyPath))
            {
                if (!File.Exists(Constants.DefaultSigningPrivateKeyPath))
                {
                    yield return ValidationMessages.PrivateKeyFile;
                }
            }
            else if (!File.Exists(privateKeyPath) || !privateKeyPath.EndsWith(Constants.PrivateKeyExtension))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
            if (!string.IsNullOrEmpty(comment) && comment.Length > 500)
            {
                yield return "Please enter a shorter comment.";
            }
            if (!string.IsNullOrEmpty(signatureFilePath) && !signatureFilePath.EndsWith(Constants.SignatureExtension))
            {
                yield return _invalidSignatureFile;
            }
            if (filePaths == null)
            {
                yield return "Please specify a file to sign.";
            }
            else if (filePaths.Length > 1 && !string.IsNullOrEmpty(signatureFilePath))
            {
                yield return "You cannot specify a signature file when signing multiple files.";
            }
        }

        public static bool Verify(char[] encodedPublicKey, string signatureFilePath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetVerifyErrors(encodedPublicKey, signatureFilePath, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetVerifyErrors(char[] encodedPublicKey, string signatureFilePath, string[] filePaths)
        {
            if (encodedPublicKey.Length != Constants.PublicKeyLength)
            {
                yield return ValidationMessages.PublicKeyString;
            }
            if (!string.IsNullOrEmpty(signatureFilePath) && (!signatureFilePath.EndsWith(Constants.SignatureExtension) || !File.Exists(signatureFilePath)))
            {
                yield return _invalidSignatureFile;
            }
            if (filePaths == null)
            {
                yield return _noFileToVerifyError;
            }
            else if (filePaths.Length > 1)
            {
                yield return _singleFileToVerifyError;
            }
        }

        public static bool Verify(string publicKeyPath, string signatureFilePath, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetVerifyErrors(publicKeyPath, signatureFilePath, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetVerifyErrors(string publicKeyPath, string signatureFilePath, string[] filePaths)
        {
            if (!File.Exists(publicKeyPath) || !publicKeyPath.EndsWith(Constants.PublicKeyExtension))
            {
                yield return ValidationMessages.PublicKeyFile;
            }
            if (!string.IsNullOrEmpty(signatureFilePath) && (!signatureFilePath.EndsWith(Constants.SignatureExtension) || !File.Exists(signatureFilePath)))
            {
                yield return _invalidSignatureFile;
            }
            if (filePaths == null)
            {
                yield return _noFileToVerifyError;
            }
            else if (filePaths.Length > 1)
            {
                yield return _singleFileToVerifyError;
            }
        }

        public static bool SignatureFile(string signatureFilePath)
        {
            IEnumerable<string> errorMessages = GetSignatureFileError(signatureFilePath);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetSignatureFileError(string signatureFilePath)
        {
            if (string.IsNullOrEmpty(signatureFilePath))
            {
                yield return "Please specify a signature file.";
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
                    yield return _invalidSignatureFile;
                }
            }
        }
    }
}
