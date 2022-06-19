/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2022 Samuel Lucas

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

using System.IO;
using System.Collections.Generic;

namespace Kryptor;

public static class SigningValidation
{
    public static bool Sign(string privateKeyPath, string comment, string[] signatureFilePaths, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetSignErrors(privateKeyPath, comment, signatureFilePaths, filePaths);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetSignErrors(string privateKeyPath, string comment, string[] signatureFilePaths, string[] filePaths)
    {
        if (string.Equals(privateKeyPath, Constants.DefaultSigningPrivateKeyPath) && !File.Exists(Constants.DefaultSigningPrivateKeyPath))
        {
            yield return ErrorMessages.NonExistentDefaultPrivateKeyFile;
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && !privateKeyPath.EndsWith(Constants.PrivateKeyExtension))
        {
            yield return ErrorMessages.InvalidPrivateKeyFile;
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && !File.Exists(privateKeyPath))
        {
            yield return ErrorMessages.NonExistentPrivateKeyFile;
        }
        if (!string.IsNullOrEmpty(comment) && comment.Length > 500)
        {
            yield return "Please enter a shorter comment. The maximum length is 500 characters.";
        }
        if (signatureFilePaths != null)
        {
            foreach (string signatureFilePath in signatureFilePaths)
            {
                if (!signatureFilePath.EndsWith(Constants.SignatureExtension)) { yield return ErrorMessages.GetFilePathError(signatureFilePath, "Please specify a .signature file."); }
                if (Directory.Exists(signatureFilePath)) { yield return ErrorMessages.GetFilePathError(signatureFilePath, "Please specify a file, not a directory."); }
            }
        }
        if (filePaths == null)
        {
            yield return "Please specify a file to sign.";
        }
        else
        {
            foreach (string filePath in filePaths)
            {
                IEnumerable<string> errorMessages = FilePathValidation.GetFileSigningError(filePath, signatureFilePaths);
                foreach (string errorMessage in errorMessages)
                {
                    yield return ErrorMessages.GetFilePathError(filePath, errorMessage);
                }
            }
        }
        if (signatureFilePaths != null && filePaths != null && signatureFilePaths.Length != filePaths.Length)
        {
            yield return "Please specify the same number of signature files and files to sign.";
        }
    }

    public static bool Verify(char[] encodedPublicKey, string[] signatureFilePaths, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetVerifyErrors(encodedPublicKey, signatureFilePaths, filePaths);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetVerifyErrors(char[] encodedPublicKey, string[] signatureFilePaths, string[] filePaths)
    {
        if (encodedPublicKey.Length != Constants.PublicKeyLength) { yield return ErrorMessages.InvalidPublicKey; }
        foreach (string errorMessage in GetVerifyFilePathsErrors(filePaths, signatureFilePaths))
        {
            yield return errorMessage;
        }
    }

    public static bool Verify(string publicKeyPath, string[] signatureFilePaths, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetVerifyErrors(publicKeyPath, signatureFilePaths, filePaths);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetVerifyErrors(string publicKeyPath, string[] signatureFilePaths, string[] filePaths)
    {
        if (!string.IsNullOrEmpty(publicKeyPath) && !publicKeyPath.EndsWith(Constants.PublicKeyExtension))
        {
            yield return ErrorMessages.InvalidPublicKeyFile;
        }
        else if (!File.Exists(publicKeyPath))
        {
            yield return ErrorMessages.NonExistentPublicKeyFile;
        }
        foreach (string errorMessage in GetVerifyFilePathsErrors(filePaths, signatureFilePaths))
        {
            yield return errorMessage;
        }
    }
    
    private static IEnumerable<string> GetVerifyFilePathsErrors(string[] filePaths, string[] signatureFilePaths)
    {
        if (filePaths == null)
        {
            yield return "Please specify a file to verify.";
        }
        else
        {
            foreach (string filePath in filePaths)
            {
                string errorMessage = FilePathValidation.GetSignatureVerifyError(filePath, signatureFilePaths == null ? filePath + Constants.SignatureExtension : string.Empty);
                if (!string.IsNullOrEmpty(errorMessage)) { yield return ErrorMessages.GetFilePathError(filePath, errorMessage); }
            }
        }
        if (signatureFilePaths != null)
        {
            if (filePaths != null && signatureFilePaths.Length != filePaths.Length) { yield return "Please specify the same number of signature files and files to verify."; }
            foreach (string signatureFilePath in signatureFilePaths)
            {
                string errorMessage = FilePathValidation.GetSignatureFileError(signatureFilePath);
                if (!string.IsNullOrEmpty(errorMessage)) { yield return ErrorMessages.GetFilePathError(signatureFilePath, errorMessage); }
            }
        }
    }
}