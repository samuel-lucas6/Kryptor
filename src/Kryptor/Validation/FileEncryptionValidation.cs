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

    public static bool FileEncryptionWithPassword(bool usePassword, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(usePassword, symmetricKey).Concat(GetEncryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionErrors(bool usePassword, string symmetricKey)
    {
        if (!usePassword && string.IsNullOrEmpty(symmetricKey)) { yield return PasswordOrSymmetricKeyError; }
        if (Path.EndsInDirectorySeparator(symmetricKey) && !Directory.Exists(symmetricKey))
        {
            yield return ErrorMessages.GetFilePathError(symmetricKey, "Please specify a valid directory for the keyfile.");
        }
        else if (File.Exists(symmetricKey) && FileHandling.GetFileLength(symmetricKey) < Constants.KeyfileLength)
        {
            yield return ErrorMessages.GetFilePathError(symmetricKey, "Please specify a keyfile that's at least 64 bytes in size.");
        }
        else if (!string.IsNullOrEmpty(symmetricKey) && Arrays.Compare(new[] {symmetricKey[^1]}, Constants.Base64Padding) && symmetricKey.Length != Constants.SymmetricKeyLength)
        {
            yield return ErrorMessages.GetKeyStringError(symmetricKey, ErrorMessages.InvalidSymmetricKey);
        }
    }

    private static IEnumerable<string> GetEncryptionFilePathErrors(string[] filePaths)
    {
        if (filePaths == null)
        {
            yield return FileOrDirectoryError;
        }
        else
        {
            foreach (string inputFilePath in filePaths)
            {
                string errorMessage = FilePathValidation.GetFileEncryptionError(inputFilePath);
                if (!string.IsNullOrEmpty(errorMessage)) { yield return ErrorMessages.GetFilePathError(inputFilePath, errorMessage); }
            }
        }
    }

    public static bool FileEncryptionWithPublicKeyFile(string privateKeyPath, string[] publicKeyPaths, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionWithPublicKeyFileErrors(privateKeyPath, publicKeyPaths).Concat(GetFileEncryptionErrors(usePassword: true, symmetricKey)).Concat(GetEncryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionWithPublicKeyFileErrors(string privateKeyPath, string[] publicKeyPaths)
    {
        foreach (string errorMessage in GetFileEncryptionErrors(privateKeyPath))
        {
            yield return errorMessage;
        }
        if (publicKeyPaths == null)
        {
            yield return ErrorMessages.NoPublicKey;
        }
        else
        {
            foreach (string publicKeyPath in publicKeyPaths)
            {
                if (!publicKeyPath.EndsWith(Constants.PublicKeyExtension))
                {
                    yield return ErrorMessages.GetFilePathError(publicKeyPath, ErrorMessages.InvalidPublicKeyFile);
                }
                else if (!File.Exists(publicKeyPath))
                {
                    yield return ErrorMessages.GetFilePathError(publicKeyPath, ErrorMessages.NonExistentPublicKeyFile);
                }
            }
        }
    }

    public static bool FileEncryptionWithPublicKeyString(string privateKeyPath, string[] encodedPublicKeys, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionWithPublicKeyStringErrors(privateKeyPath, encodedPublicKeys).Concat(GetFileEncryptionErrors(usePassword: true, symmetricKey)).Concat(GetEncryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionWithPublicKeyStringErrors(string privateKeyPath, string[] encodedPublicKeys)
    {
        foreach (string errorMessage in GetFileEncryptionErrors(privateKeyPath))
        {
            yield return errorMessage;
        }
        if (encodedPublicKeys == null)
        {
            yield return ErrorMessages.NoPublicKey;
        }
        else
        {
            foreach (string encodedPublicKey in encodedPublicKeys)
            {
                if (encodedPublicKey.EndsWith(Constants.PublicKeyExtension))
                {
                    yield return ErrorMessages.GetFilePathError(encodedPublicKey, "Please specify only public key strings or only public key files.");
                }
                else if (encodedPublicKey.Length != Constants.PublicKeyLength)
                {
                    yield return ErrorMessages.GetKeyStringError(encodedPublicKey, ErrorMessages.InvalidPublicKey);
                }
            }
        }
    }

    public static bool FileEncryptionWithPrivateKey(string privateKeyPath, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetFileEncryptionErrors(usePassword: true, symmetricKey)).Concat(GetEncryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath)
    {
        if (string.Equals(privateKeyPath, Constants.DefaultEncryptionPrivateKeyPath) && !File.Exists(Constants.DefaultEncryptionPrivateKeyPath))
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
    }

    public static bool FileDecryptionWithPassword(bool usePassword, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionErrors(usePassword, symmetricKey).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileDecryptionErrors(bool usePassword, string symmetricKey)
    {
        if (!usePassword && string.IsNullOrEmpty(symmetricKey)) { yield return PasswordOrSymmetricKeyError; }
        if (!string.IsNullOrEmpty(symmetricKey) && Arrays.Compare(new[] {symmetricKey[^1]}, Constants.Base64Padding) && symmetricKey.Length != Constants.SymmetricKeyLength)
        {
            yield return ErrorMessages.GetKeyStringError(symmetricKey, ErrorMessages.InvalidSymmetricKey);
        }
        else if (!string.IsNullOrEmpty(symmetricKey) && !Arrays.Compare(new[] {symmetricKey[^1]}, Constants.Base64Padding) && !File.Exists(symmetricKey))
        {
            yield return ErrorMessages.GetFilePathError(symmetricKey, "Please specify a valid symmetric key string or a keyfile that exists.");
        }
    }

    private static IEnumerable<string> GetDecryptionFilePathErrors(string[] filePaths)
    {
        if (filePaths == null)
        {
            yield return FileOrDirectoryError;
        }
        else
        {
            foreach (string inputFilePath in filePaths)
            {
                string errorMessage = FilePathValidation.GetFileDecryptionError(inputFilePath);
                if (!string.IsNullOrEmpty(errorMessage)) { yield return ErrorMessages.GetFilePathError(inputFilePath, errorMessage); }
            }
        }
    }

    public static bool FileDecryptionWithPublicKeyFile(string privateKeyPath, string[] publicKeyPaths, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionWithPublicKeyFileErrors(privateKeyPath, publicKeyPaths).Concat(GetFileDecryptionErrors(usePassword: true, symmetricKey)).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }
    
    private static IEnumerable<string> GetFileDecryptionWithPublicKeyFileErrors(string privateKeyPath, string[] publicKeyPaths)
    {
        foreach (string errorMessage in GetFileEncryptionErrors(privateKeyPath))
        {
            yield return errorMessage;
        }
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
    }

    public static bool FileDecryptionWithPublicKeyString(string privateKeyPath, string[] encodedPublicKeys, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionWithPublicKeyStringErrors(privateKeyPath, encodedPublicKeys).Concat(GetFileDecryptionErrors(usePassword: true, symmetricKey)).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }
    
    private static IEnumerable<string> GetFileDecryptionWithPublicKeyStringErrors(string privateKeyPath, string[] encodedPublicKeys)
    {
        foreach (string errorMessage in GetFileEncryptionErrors(privateKeyPath))
        {
            yield return errorMessage;
        }
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
    }

    public static bool FileDecryptionWithPrivateKey(string privateKeyPath, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetFileDecryptionErrors(usePassword: true, symmetricKey)).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }
}