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
using System.Linq;
using System.Collections.Generic;

namespace Kryptor;

public static class FileEncryptionValidation
{
    private const string FileOrDirectoryError = "Please specify a file/directory.";
    private const string PasswordOrSymmetricKeyError = "Please specify whether to use a password and/or symmetric key.";
    private static readonly string TooManyRecipients = $"Please specify no more than {Constants.MaxRecipients} public keys.";
    private static readonly char[] IllegalFileNameChars = {
        '\"', '<', '>', '|', '\0',
        (char) 1, (char) 2, (char) 3, (char) 4, (char) 5, (char) 6, (char) 7, (char) 8, (char) 9, (char) 10,
        (char) 11, (char) 12, (char) 13, (char) 14, (char) 15, (char) 16, (char) 17, (char) 18, (char) 19, (char) 20,
        (char) 21, (char) 22, (char) 23, (char) 24, (char) 25, (char) 26, (char) 27, (char) 28, (char) 29, (char) 30,
        (char) 31, ':', '*', '?', '\\', '/'
    };

    public static void FileEncryptionWithPassword(bool usePassword, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(usePassword, symmetricKey).Concat(GetEncryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionErrors(bool usePassword, string symmetricKey)
    {
        if (!usePassword && string.IsNullOrEmpty(symmetricKey)) {
            yield return PasswordOrSymmetricKeyError;
        }
        if (Path.EndsInDirectorySeparator(symmetricKey) && !Directory.Exists(symmetricKey)) {
            yield return ErrorMessages.GetFilePathError(symmetricKey, "Please specify a valid directory for the keyfile.");
        }
        else if (File.Exists(symmetricKey) && new FileInfo(symmetricKey).Length < Constants.KeyfileLength) {
            yield return ErrorMessages.GetFilePathError(symmetricKey, "Please specify a keyfile that's at least 64 bytes in size.");
        }
        else if (!string.IsNullOrEmpty(symmetricKey) && symmetricKey.EndsWith(Constants.Base64Padding) && symmetricKey.Length != Constants.SymmetricKeyLength) {
            yield return ErrorMessages.GetKeyStringError(symmetricKey, ErrorMessages.InvalidSymmetricKey);
        }
    }

    private static IEnumerable<string> GetEncryptionFilePathErrors(string[] filePaths)
    {
        if (filePaths == null) {
            yield return FileOrDirectoryError;
        }
        else {
            foreach (string inputFilePath in filePaths) {
                string errorMessage = GetFileEncryptionError(inputFilePath);
                if (!string.IsNullOrEmpty(errorMessage)) {
                    yield return ErrorMessages.GetFilePathError(inputFilePath, errorMessage);
                }
            }
        }
    }
    
    private static string GetFileEncryptionError(string inputFilePath)
    {
        if (Path.GetFileName(Path.TrimEndingDirectorySeparator(inputFilePath)).IndexOfAny(IllegalFileNameChars) != -1) { return "This file/directory name contains illegal characters for Windows, Linux, and/or macOS.";}
        if (Directory.Exists(inputFilePath)) { return FileHandling.IsDirectoryEmpty(inputFilePath) ? ErrorMessages.DirectoryEmpty : null; }
        return !File.Exists(inputFilePath) ? ErrorMessages.FileOrDirectoryDoesNotExist : null;
    }

    public static void FileEncryptionWithPublicKeyFile(string privateKeyPath, string[] publicKeyPaths, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionWithPublicKeyFileErrors(privateKeyPath, publicKeyPaths).Concat(GetFileEncryptionErrors(usePassword: true, symmetricKey)).Concat(GetEncryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionWithPublicKeyFileErrors(string privateKeyPath, string[] publicKeyPaths)
    {
        foreach (string errorMessage in GetFileEncryptionErrors(privateKeyPath)) {
            yield return errorMessage;
        }
        if (publicKeyPaths == null) {
            yield return ErrorMessages.NoPublicKey;
        }
        else if (publicKeyPaths.Length > Constants.MaxRecipients) {
            yield return TooManyRecipients;
        }
        else {
            foreach (string publicKeyPath in publicKeyPaths) {
                if (!publicKeyPath.EndsWith(Constants.PublicKeyExtension)) {
                    yield return ErrorMessages.GetFilePathError(publicKeyPath, ErrorMessages.InvalidPublicKeyFile);
                }
                else if (!File.Exists(publicKeyPath)) {
                    yield return ErrorMessages.GetFilePathError(publicKeyPath, ErrorMessages.NonExistentPublicKeyFile);
                }
            }
        }
    }

    public static void FileEncryptionWithPublicKeyString(string privateKeyPath, string[] encodedPublicKeys, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionWithPublicKeyStringErrors(privateKeyPath, encodedPublicKeys).Concat(GetFileEncryptionErrors(usePassword: true, symmetricKey)).Concat(GetEncryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionWithPublicKeyStringErrors(string privateKeyPath, string[] encodedPublicKeys)
    {
        foreach (string errorMessage in GetFileEncryptionErrors(privateKeyPath)) {
            yield return errorMessage;
        }
        if (encodedPublicKeys == null) {
            yield return ErrorMessages.NoPublicKey;
        }
        else if (encodedPublicKeys.Length > Constants.MaxRecipients) {
            yield return TooManyRecipients;
        }
        else {
            foreach (string encodedPublicKey in encodedPublicKeys) {
                if (encodedPublicKey.EndsWith(Constants.PublicKeyExtension)) {
                    yield return ErrorMessages.GetFilePathError(encodedPublicKey, "Please specify only public key strings or only public key files.");
                }
                else if (encodedPublicKey.Length != Constants.PublicKeyLength) {
                    yield return ErrorMessages.GetKeyStringError(encodedPublicKey, ErrorMessages.InvalidPublicKey);
                }
            }
        }
    }

    public static void FileEncryptionWithPrivateKey(string privateKeyPath, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetFileEncryptionErrors(usePassword: true, symmetricKey)).Concat(GetEncryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath)
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

    public static void FileDecryptionWithPassword(bool usePassword, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionErrors(usePassword, symmetricKey).Concat(GetDecryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileDecryptionErrors(bool usePassword, string symmetricKey)
    {
        if (!usePassword && string.IsNullOrEmpty(symmetricKey)) {
            yield return PasswordOrSymmetricKeyError;
        }
        if (!string.IsNullOrEmpty(symmetricKey) && symmetricKey.EndsWith(Constants.Base64Padding) && symmetricKey.Length != Constants.SymmetricKeyLength) {
            yield return ErrorMessages.GetKeyStringError(symmetricKey, ErrorMessages.InvalidSymmetricKey);
        }
        else if (!string.IsNullOrEmpty(symmetricKey) && !symmetricKey.EndsWith(Constants.Base64Padding) && !File.Exists(symmetricKey)) {
            yield return ErrorMessages.GetFilePathError(symmetricKey, "Please specify a valid symmetric key string or a keyfile that exists.");
        }
    }

    private static IEnumerable<string> GetDecryptionFilePathErrors(string[] filePaths)
    {
        if (filePaths == null) {
            yield return FileOrDirectoryError;
        }
        else {
            foreach (string inputFilePath in filePaths) {
                string errorMessage = GetFileDecryptionError(inputFilePath);
                if (!string.IsNullOrEmpty(errorMessage)) {
                    yield return ErrorMessages.GetFilePathError(inputFilePath, errorMessage);
                }
            }
        }
    }
    
    private static string GetFileDecryptionError(string inputFilePath)
    {
        if (Directory.Exists(inputFilePath)) { return FileHandling.IsDirectoryEmpty(inputFilePath) ? ErrorMessages.DirectoryEmpty : null; }
        return !File.Exists(inputFilePath) ? ErrorMessages.FileOrDirectoryDoesNotExist : null;
    }

    public static void FileDecryptionWithPublicKeyFile(string privateKeyPath, string[] publicKeyPaths, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionWithPublicKeyFileErrors(privateKeyPath, publicKeyPaths).Concat(GetFileDecryptionErrors(usePassword: true, symmetricKey)).Concat(GetDecryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
    }
    
    private static IEnumerable<string> GetFileDecryptionWithPublicKeyFileErrors(string privateKeyPath, string[] publicKeyPaths)
    {
        foreach (string errorMessage in GetFileEncryptionErrors(privateKeyPath)) {
            yield return errorMessage;
        }
        if (publicKeyPaths == null) {
            yield return ErrorMessages.NoPublicKey;
        }
        else if (publicKeyPaths.Length > 1) {
            yield return ErrorMessages.MultiplePublicKeys;
        }
        else if (!publicKeyPaths[0].EndsWith(Constants.PublicKeyExtension)) {
            yield return ErrorMessages.GetFilePathError(publicKeyPaths[0], ErrorMessages.InvalidPublicKeyFile);
        }
        else if (!File.Exists(publicKeyPaths[0])) {
            yield return ErrorMessages.GetFilePathError(publicKeyPaths[0], ErrorMessages.NonExistentPublicKeyFile);
        }
    }

    public static void FileDecryptionWithPublicKeyString(string privateKeyPath, string[] encodedPublicKeys, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionWithPublicKeyStringErrors(privateKeyPath, encodedPublicKeys).Concat(GetFileDecryptionErrors(usePassword: true, symmetricKey)).Concat(GetDecryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
    }
    
    private static IEnumerable<string> GetFileDecryptionWithPublicKeyStringErrors(string privateKeyPath, string[] encodedPublicKeys)
    {
        foreach (string errorMessage in GetFileEncryptionErrors(privateKeyPath)) {
            yield return errorMessage;
        }
        if (encodedPublicKeys == null) {
            yield return ErrorMessages.NoPublicKey;
        }
        else if (encodedPublicKeys.Length > 1) {
            yield return ErrorMessages.MultiplePublicKeys;
        }
        else if (encodedPublicKeys[0].Length != Constants.PublicKeyLength) {
            yield return ErrorMessages.GetKeyStringError(encodedPublicKeys[0], ErrorMessages.InvalidPublicKey);
        }
    }

    public static void FileDecryptionWithPrivateKey(string privateKeyPath, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetFileDecryptionErrors(usePassword: true, symmetricKey)).Concat(GetDecryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
    }
}