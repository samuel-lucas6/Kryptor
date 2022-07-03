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
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.InvalidPrivateKeyFile);
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && !File.Exists(privateKeyPath))
        {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.NonExistentPrivateKeyFile);
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

    public static bool VerifyWithPublicKeyString(string[] encodedPublicKeys, string[] signatureFilePaths, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetVerifyWithPublicKeyStringErrors(encodedPublicKeys, signatureFilePaths, filePaths);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetVerifyWithPublicKeyStringErrors(string[] encodedPublicKeys, string[] signatureFilePaths, string[] filePaths)
    {
        if (encodedPublicKeys == null)
        {
            yield return ErrorMessages.NoPublicKey;
        }
        else if (encodedPublicKeys.Length > 1)
        {
            yield return ErrorMessages.MultiplePublicKeys;
        }
        else if (encodedPublicKeys[0].Length != Constants.PublicKeyLength)
        {
            yield return ErrorMessages.GetKeyStringError(encodedPublicKeys[0], ErrorMessages.InvalidPublicKey);
        }
        foreach (string errorMessage in GetVerifyFilePathsErrors(filePaths, signatureFilePaths))
        {
            yield return errorMessage;
        }
    }

    public static bool VerifyWithPublicKeyFile(string[] publicKeyPaths, string[] signatureFilePaths, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetVerifyWithPublicKeyFileErrors(publicKeyPaths, signatureFilePaths, filePaths);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetVerifyWithPublicKeyFileErrors(string[] publicKeyPaths, string[] signatureFilePaths, string[] filePaths)
    {
        if (publicKeyPaths == null)
        {
            yield return ErrorMessages.NoPublicKey;
        }
        else if (publicKeyPaths.Length > 1)
        {
            yield return ErrorMessages.MultiplePublicKeys;
        }
        else if (!publicKeyPaths[0].EndsWith(Constants.PublicKeyExtension))
        {
            yield return ErrorMessages.GetFilePathError(publicKeyPaths[0], ErrorMessages.InvalidPublicKeyFile);
        }
        else if (!File.Exists(publicKeyPaths[0]))
        {
            yield return ErrorMessages.GetFilePathError(publicKeyPaths[0], ErrorMessages.NonExistentPublicKeyFile);
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