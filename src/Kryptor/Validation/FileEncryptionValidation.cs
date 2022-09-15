﻿/*
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
using System.Linq;
using System.Collections.Generic;

namespace Kryptor;

public static class FileEncryptionValidation
{
    private const string FileOrDirectoryError = "Please specify a file/directory.";
    private static readonly char[] IllegalFileNameChars = {
        '\"', '<', '>', '|', '\0',
        (char) 1, (char) 2, (char) 3, (char) 4, (char) 5, (char) 6, (char) 7, (char) 8, (char) 9, (char) 10,
        (char) 11, (char) 12, (char) 13, (char) 14, (char) 15, (char) 16, (char) 17, (char) 18, (char) 19, (char) 20,
        (char) 21, (char) 22, (char) 23, (char) 24, (char) 25, (char) 26, (char) 27, (char) 28, (char) 29, (char) 30,
        (char) 31, ':', '*', '?', '\\', '/'
    };
    
    public static IEnumerable<string> GetEncryptionErrors(string symmetricKey)
    {
        if (Path.EndsInDirectorySeparator(symmetricKey) && !Directory.Exists(symmetricKey)) {
            yield return ErrorMessages.GetFilePathError(symmetricKey, "Please specify a valid directory for the keyfile.");
        }
        else if (File.Exists(symmetricKey) && new FileInfo(symmetricKey).Length < Constants.KeyfileLength) {
            yield return ErrorMessages.GetFilePathError(symmetricKey, $"Please specify a keyfile that's at least {Constants.KeyfileLength} bytes in size.");
        }
        else if (!string.IsNullOrEmpty(symmetricKey) && symmetricKey.EndsWith(Constants.Base64Padding) && symmetricKey.Length != Constants.SymmetricKeyLength) {
            yield return ErrorMessages.GetKeyStringError(symmetricKey, ErrorMessages.InvalidSymmetricKey);
        }
    }

    public static IEnumerable<string> GetEncryptionErrors(string privateKeyPath, string[] publicKeys, string symmetricKey)
    {
        foreach (string errorMessage in GetPrivateKeyErrors(privateKeyPath)) {
            yield return errorMessage;
        }
        
        if (publicKeys == null) {
            yield return ErrorMessages.NoPublicKey;
        }
        else if (publicKeys.Length > Constants.MaxRecipients) {
            yield return $"Please specify no more than {Constants.MaxRecipients} public keys.";
        }
        else {
            foreach (string publicKey in publicKeys) {
                if (!publicKey.EndsWith(Constants.PublicKeyExtension)) {
                    if (File.Exists(publicKey) || !string.IsNullOrEmpty(Path.GetExtension(publicKey))) {
                        yield return ErrorMessages.GetFilePathError(publicKey, ErrorMessages.InvalidPublicKeyFile);
                    }
                    else if (string.IsNullOrEmpty(Path.GetExtension(publicKey)) && publicKey.Length != Constants.PublicKeyLength) {
                        yield return ErrorMessages.GetKeyStringError(publicKey, ErrorMessages.InvalidPublicKey);
                    }
                }
                else if (!File.Exists(publicKey)) {
                    yield return ErrorMessages.GetFilePathError(publicKey, ErrorMessages.NonExistentPublicKeyFile);
                }
            }
            if (publicKeys.Any(key => !string.IsNullOrEmpty(Path.GetExtension(key))) && publicKeys.Any(key => string.IsNullOrEmpty(Path.GetExtension(key)))) {
                yield return "Please specify only public key strings or only public key files.";
            }
        }
        
        foreach (string errorMessage in GetEncryptionErrors(symmetricKey)) {
            yield return errorMessage;
        }
    }

    public static IEnumerable<string> GetEncryptionErrors(string privateKeyPath, string symmetricKey)
    {
        foreach (string errorMessage in GetPrivateKeyErrors(privateKeyPath)) {
            yield return errorMessage;
        }
        
        foreach (string errorMessage in GetEncryptionErrors(symmetricKey)) {
            yield return errorMessage;
        }
    }
    
    private static IEnumerable<string> GetPrivateKeyErrors(string privateKeyPath)
    {
        if (string.Equals(privateKeyPath, Constants.DefaultEncryptionPrivateKeyPath) && !File.Exists(Constants.DefaultEncryptionPrivateKeyPath)) {
            yield return ErrorMessages.NonExistentDefaultPrivateKeyFile;
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && !privateKeyPath.EndsWith(Constants.PrivateKeyExtension)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.InvalidPrivateKeyFile);
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && !File.Exists(privateKeyPath)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.NonExistentPrivateKeyFile);
        }
    }
    
    public static IEnumerable<string> GetEncryptionFilePathErrors(string[] filePaths)
    {
        if (filePaths == null) {
            yield return FileOrDirectoryError;
        }
        else {
            foreach (string inputFilePath in filePaths) {
                if (Path.GetFileName(Path.TrimEndingDirectorySeparator(inputFilePath)).IndexOfAny(IllegalFileNameChars) != -1) {
                    yield return ErrorMessages.GetFilePathError(inputFilePath, "This file/directory name contains illegal characters for cross-platform decryption.");
                }
                else if (Directory.Exists(inputFilePath)) {
                    bool? isEmpty = FileHandling.IsDirectoryEmpty(inputFilePath);
                    if (isEmpty == true) {
                        yield return ErrorMessages.GetFilePathError(inputFilePath, ErrorMessages.DirectoryEmpty);
                    }
                    else if (isEmpty == null) {
                        yield return ErrorMessages.GetFilePathError(inputFilePath, ErrorMessages.UnableToAccessDirectory);
                    }
                }
                else if (!File.Exists(inputFilePath)) {
                    yield return ErrorMessages.GetFilePathError(inputFilePath, ErrorMessages.FileOrDirectoryDoesNotExist);
                }
            }
        }
    }

    public static IEnumerable<string> GetDecryptionErrors(string symmetricKey)
    {
        if (!string.IsNullOrEmpty(symmetricKey) && symmetricKey.EndsWith(Constants.Base64Padding) && symmetricKey.Length != Constants.SymmetricKeyLength) {
            yield return ErrorMessages.GetKeyStringError(symmetricKey, ErrorMessages.InvalidSymmetricKey);
        }
        else if (!string.IsNullOrEmpty(symmetricKey) && !symmetricKey.EndsWith(Constants.Base64Padding) && !File.Exists(symmetricKey)) {
            yield return ErrorMessages.GetFilePathError(symmetricKey, "Please specify a valid symmetric key string or a keyfile that exists.");
        }
    }
    
    public static IEnumerable<string> GetDecryptionErrors(string privateKeyPath, string[] publicKeys, string symmetricKey)
    {
        foreach (string errorMessage in GetPrivateKeyErrors(privateKeyPath)) {
            yield return errorMessage;
        }
        
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

        foreach (string errorMessage in GetDecryptionErrors(symmetricKey)) {
            yield return errorMessage;
        }
    }
    
    public static IEnumerable<string> GetDecryptionErrors(string privateKeyPath, string symmetricKey)
    {
        foreach (string errorMessage in GetPrivateKeyErrors(privateKeyPath)) {
            yield return errorMessage;
        }
        
        foreach (string errorMessage in GetDecryptionErrors(symmetricKey)) {
            yield return errorMessage;
        }
    }
    
    public static IEnumerable<string> GetDecryptionFilePathErrors(string[] filePaths)
    {
        if (filePaths == null) {
            yield return FileOrDirectoryError;
        }
        else {
            foreach (string inputFilePath in filePaths) {
                if (Directory.Exists(inputFilePath)) {
                    bool? isEmpty = FileHandling.IsDirectoryEmpty(inputFilePath);
                    if (isEmpty == true) {
                        yield return ErrorMessages.GetFilePathError(inputFilePath, ErrorMessages.DirectoryEmpty);
                    }
                    else if (isEmpty == null) {
                        yield return ErrorMessages.GetFilePathError(inputFilePath, ErrorMessages.UnableToAccessDirectory);
                    }
                }
                else if (!File.Exists(inputFilePath)) {
                    yield return ErrorMessages.GetFilePathError(inputFilePath, ErrorMessages.FileOrDirectoryDoesNotExist);
                }
            }
        }
    }
}