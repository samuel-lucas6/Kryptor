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

using System;
using System.IO;
using System.Collections.Generic;

namespace Kryptor;

public static class SigningValidation
{
    private const string InvalidSignatureFile = "Please specify a signature file with a valid format.";
    private const string SignatureFileInaccessible = "Unable to access the signature file.";
    
    public static IEnumerable<string> GetSignErrors(string privateKeyPath, string comment, string[] signaturePaths, string[] filePaths)
    {
        if (string.Equals(privateKeyPath, Constants.DefaultSigningPrivateKeyPath) && !File.Exists(Constants.DefaultSigningPrivateKeyPath)) {
            yield return ErrorMessages.NonExistentDefaultPrivateKeyFile;
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && !privateKeyPath.EndsWith(Constants.PrivateKeyExtension)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.InvalidPrivateKeyFile);
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && !File.Exists(privateKeyPath)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.NonExistentPrivateKeyFile);
        }
        
        if (!string.IsNullOrEmpty(comment) && comment.Length > 500) {
            yield return "Please enter a shorter comment. The maximum length is 500 characters.";
        }
        
        if (signaturePaths != null) {
            foreach (string signaturePath in signaturePaths) {
                if (!signaturePath.EndsWith(Constants.SignatureExtension)) {
                    yield return ErrorMessages.GetFilePathError(signaturePath, "Please specify a .signature file.");
                }
                if (Directory.Exists(signaturePath)) {
                    yield return ErrorMessages.GetFilePathError(signaturePath, "Please specify a file, not a directory.");
                }
            }
        }
        
        if (filePaths == null) {
            yield return "Please specify a file to sign.";
        }
        else {
            foreach (string filePath in filePaths) {
                if (Directory.Exists(filePath)) {
                    if (FileHandling.IsDirectoryEmpty(filePath)) {
                        yield return ErrorMessages.GetFilePathError(filePath, ErrorMessages.DirectoryEmpty);
                    }
                    if (signaturePaths != null) {
                        yield return ErrorMessages.GetFilePathError(filePath, "You cannot specify signature files when signing a directory.");
                    }
                }
                else if (!File.Exists(filePath)) {
                    yield return ErrorMessages.GetFilePathError(filePath, ErrorMessages.FileOrDirectoryDoesNotExist);
                }
            }
        }
        if (signaturePaths != null && filePaths != null && signaturePaths.Length != filePaths.Length) {
            yield return "Please specify the same number of signature files and files to sign.";
        }
    }

    public static IEnumerable<string> GetVerifyErrors(string[] publicKeys, string[] signaturePaths, string[] filePaths)
    {
        if (publicKeys == null) {
            yield return ErrorMessages.NoPublicKey;
        }
        else if (publicKeys.Length > 1) {
            yield return ErrorMessages.MultiplePublicKeys;
        }
        else if (!publicKeys[0].EndsWith(Constants.PublicKeyExtension)) {
            if (File.Exists(publicKeys[0]) || !string.IsNullOrEmpty(Path.GetExtension(publicKeys[0]))) {
                yield return ErrorMessages.GetFilePathError(publicKeys[0], ErrorMessages.InvalidPublicKeyFile);
            }
            else if (string.IsNullOrEmpty(Path.GetExtension(publicKeys[0])) && publicKeys[0].Length != Constants.PublicKeyLength) {
                yield return ErrorMessages.GetKeyStringError(publicKeys[0], ErrorMessages.InvalidPublicKey);
            }
        }
        else if (!File.Exists(publicKeys[0])) {
            yield return ErrorMessages.GetFilePathError(publicKeys[0], ErrorMessages.NonExistentPublicKeyFile);
        }

        if (filePaths == null) {
            yield return "Please specify a file to verify.";
        }
        else {
            foreach (string filePath in filePaths) {
                string errorMessage = GetVerifyFileError(filePath, signaturePaths == null ? filePath + Constants.SignatureExtension : string.Empty);
                if (!string.IsNullOrEmpty(errorMessage)) {
                    yield return ErrorMessages.GetFilePathError(filePath, errorMessage);
                }
            }
        }

        if (signaturePaths == null) {
            yield break;
        }
        foreach (string signaturePath in signaturePaths) {
            string errorMessage = GetSignatureFileError(signaturePath);
            if (!string.IsNullOrEmpty(errorMessage)) {
                yield return ErrorMessages.GetFilePathError(signaturePath, errorMessage);
            }
        }
        if (filePaths != null && signaturePaths.Length != filePaths.Length) {
            yield return "Please specify the same number of signature files and files to verify.";
        }
    }

    private static string GetVerifyFileError(string filePath, string signatureFilePath)
    {
        if (filePath.EndsWith(Constants.SignatureExtension)) { return "Please specify the file to verify, not the signature file."; }
        if (Directory.Exists(filePath)) { return "Please only specify files, not directories."; }
        if (!File.Exists(filePath)) { return "This file doesn't exist."; }
        if (string.IsNullOrEmpty(signatureFilePath)) { return null; }
        if (!File.Exists(signatureFilePath)) { return "Unable to find the signature file. Please specify it manually using -t|--signature."; }
        bool? validMagicBytes = IsValidSignatureFile(signatureFilePath, out bool? validVersion);
        if (validMagicBytes == null) { return SignatureFileInaccessible; }
        if (validMagicBytes == false) { return "The signature file that was found doesn't have a valid format."; }
        return validVersion == false ? "The signature file that was found doesn't have a valid version." : null;
    }

    private static string GetSignatureFileError(string signatureFilePath)
    {
        if (!signatureFilePath.EndsWith(Constants.SignatureExtension)) { return InvalidSignatureFile; }
        if (!File.Exists(signatureFilePath)) { return "Please specify a signature file that exists."; }
        bool? validMagicBytes = IsValidSignatureFile(signatureFilePath, out bool? validVersion);
        if (validMagicBytes == null) { return SignatureFileInaccessible; }
        if (validMagicBytes == false) { return InvalidSignatureFile; }
        return validVersion == false ? "This signature file doesn't have a valid version." : null;
    }
    
    private static bool? IsValidSignatureFile(string filePath, out bool? validVersion)
    {
        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 0);
            Span<byte> magicBytes = stackalloc byte[Constants.SignatureMagicBytes.Length];
            fileStream.Read(magicBytes);
            Span<byte> version = stackalloc byte[Constants.SignatureVersion.Length];
            fileStream.Read(version);
            validVersion = version.SequenceEqual(Constants.SignatureVersion);
            return magicBytes.SequenceEqual(Constants.SignatureMagicBytes);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            validVersion = null;
            return null;
        }
    }
}