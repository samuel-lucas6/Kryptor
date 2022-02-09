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
    private const string InvalidSignatureFile = "Please specify a valid signature file.";
    private const string NonExistentSignatureFile = "Please specify a signature file that exists.";
    private const string SingleFileToVerifyError = "Please specify a single file to verify.";

    public static bool Sign(string privateKeyPath, string comment, string signatureFilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetSignErrors(privateKeyPath, comment, signatureFilePath, filePaths);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetSignErrors(string privateKeyPath, string comment, string signatureFilePath, string[] filePaths)
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
        if (!string.IsNullOrEmpty(signatureFilePath) && !signatureFilePath.EndsWith(Constants.SignatureExtension))
        {
            yield return InvalidSignatureFile;
        }
        if (filePaths == null)
        {
            yield return ErrorMessages.NoFileToSign;
        }
        else if (filePaths.Length > 1 && !string.IsNullOrEmpty(signatureFilePath))
        {
            yield return "You cannot specify a signature file when signing multiple files.";
        }
        else
        {
            foreach (string inputFilePath in filePaths)
            {
                string errorMessage = FilePathValidation.GetFileSigningError(inputFilePath);
                if (!string.IsNullOrEmpty(errorMessage)) { yield return ErrorMessages.GetFilePathError(inputFilePath, errorMessage); }
            }
        }
    }

    public static bool Verify(char[] encodedPublicKey, string signatureFilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetVerifyErrors(encodedPublicKey, signatureFilePath, filePaths);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetVerifyErrors(char[] encodedPublicKey, string signatureFilePath, string[] filePaths)
    {
        if (encodedPublicKey.Length != Constants.PublicKeyLength) { yield return ErrorMessages.InvalidPublicKey; }
        foreach (string errorMessage in GetSignatureFileErrors(signatureFilePath))
        {
            yield return errorMessage;
        }
        foreach (string errorMessage in GetVerifyFilePathsErrors(filePaths))
        {
            yield return errorMessage;
        }
    }

    public static bool Verify(string publicKeyPath, string signatureFilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetVerifyErrors(publicKeyPath, signatureFilePath, filePaths);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetVerifyErrors(string publicKeyPath, string signatureFilePath, string[] filePaths)
    {
        if (!string.IsNullOrEmpty(publicKeyPath) && !publicKeyPath.EndsWith(Constants.PublicKeyExtension))
        {
            yield return ErrorMessages.InvalidPublicKeyFile;
        }
        else if (!File.Exists(publicKeyPath))
        {
            yield return ErrorMessages.NonExistentPublicKeyFile;
        }
        foreach (string errorMessage in GetSignatureFileErrors(signatureFilePath))
        {
            yield return errorMessage;
        }
        foreach (string errorMessage in GetVerifyFilePathsErrors(filePaths))
        {
            yield return errorMessage;
        }
    }

    private static IEnumerable<string> GetSignatureFileErrors(string signatureFilePath)
    {
        if (string.IsNullOrEmpty(signatureFilePath))
        {
            yield return "Please specify a signature file.";
        }
        else if (!signatureFilePath.EndsWith(Constants.SignatureExtension))
        {
            yield return InvalidSignatureFile;
        }
        else if (!File.Exists(signatureFilePath))
        {
            yield return NonExistentSignatureFile;
        }
        else
        {
            bool? validMagicBytes = FileHandling.IsSignatureFile(signatureFilePath);
            if (validMagicBytes == null) { yield return "Unable to access the signature file."; }
            if (validMagicBytes == false) { yield return InvalidSignatureFile; }
        }
    }

    private static IEnumerable<string> GetVerifyFilePathsErrors(string[] filePaths)
    {
        if (filePaths == null)
        {
            yield return ErrorMessages.NoFileToVerify;
        }
        else if (filePaths.Length > 1)
        {
            yield return SingleFileToVerifyError;
        }
        else
        {
            string errorMessage = FilePathValidation.GetSignatureVerifyError(filePaths[0]);
            if (!string.IsNullOrEmpty(errorMessage)) { yield return ErrorMessages.GetFilePathError(filePaths[0], errorMessage); }
        }
    }
}