using System;
using System.Security.Cryptography;

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
    public static class FileSigning
    {
        private const string _defaultComment = "This file has not been tampered with.";

        public static void SignEachFile(string[] filePaths, string signatureFilePath, string comment, bool preHash, byte[] privateKey)
        {
            privateKey = PrivateKey.Decrypt(privateKey);
            if (privateKey == null) { return; }
            if (string.IsNullOrEmpty(comment)) { comment = _defaultComment; }
            foreach (string filePath in filePaths)
            {
                try
                {
                    DigitalSignatures.SignFile(filePath, signatureFilePath, comment, preHash, privateKey);
                    DisplayMessage.FilePathMessage(filePath, "File signed successfully.");
                }
                catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
                {
                    DisplayMessage.FilePathException(filePath, ex.GetType().Name, "Unable to create signature.");
                }
            }
            CryptographicOperations.ZeroMemory(privateKey);
        }
       
        public static void VerifyFile(string signatureFilePath, string filePath, byte[] publicKey)
        {
            try
            {
                bool validSignature = DigitalSignatures.VerifySignature(signatureFilePath, filePath, publicKey, out string comment);
                if (!validSignature)
                {
                    DisplayMessage.FilePathMessage(filePath, "Bad signature.");
                    return;
                }
                DisplayMessage.FilePathMessage(filePath, "Good signature.");
                DisplayMessage.Message($"Authenticated comment: {comment}");
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException)
                {
                    DisplayMessage.FilePathMessage(signatureFilePath, ex.Message);
                    return;
                }
                DisplayMessage.Exception(ex.GetType().Name, "Unable to verify signature.");
            }
        }
    }
}
