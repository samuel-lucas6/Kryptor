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
    private const string FileOrFolderError = "Please specify a file/folder.";
    private const string PasswordOrKeyfileError = "Please specify whether to use a password and/or keyfile.";

    public static bool FileEncryptionWithPassword(bool usePassword, string keyfilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(usePassword, keyfilePath).Concat(GetEncryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionErrors(bool usePassword, string keyfilePath)
    {
        if (!usePassword && string.IsNullOrEmpty(keyfilePath)) { yield return PasswordOrKeyfileError; }
        if (Path.EndsInDirectorySeparator(keyfilePath) && !Directory.Exists(keyfilePath))
        {
            yield return "Please specify a valid directory for the keyfile.";
        }
        else if (File.Exists(keyfilePath) && FileHandling.GetFileLength(keyfilePath) < Constants.KeyfileLength)
        {
            yield return "Please specify a keyfile that's at least 64 bytes in size.";
        }
    }

    private static IEnumerable<string> GetEncryptionFilePathErrors(string[] filePaths)
    {
        if (filePaths == null)
        {
            yield return FileOrFolderError;
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

    public static bool FileEncryptionWithPublicKeyFile(string privateKeyPath, string[] publicKeyPaths, string keyfilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionWithPublicKeyFileErrors(privateKeyPath, publicKeyPaths).Concat(GetFileEncryptionErrors(usePassword: true, keyfilePath)).Concat(GetEncryptionFilePathErrors(filePaths));
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
            yield return ErrorMessages.PublicKey;
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

    public static bool FileEncryptionWithPublicKeyString(string privateKeyPath, string[] encodedPublicKeys, string keyfilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionWithPublicKeyStringErrors(privateKeyPath, encodedPublicKeys).Concat(GetFileEncryptionErrors(usePassword: true, keyfilePath)).Concat(GetEncryptionFilePathErrors(filePaths));
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
            yield return ErrorMessages.PublicKey;
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
                    yield return ErrorMessages.GetFilePathError(encodedPublicKey, ErrorMessages.InvalidPublicKey);
                }
            }
        }
    }

    public static bool FileEncryptionWithPrivateKey(string privateKeyPath, string keyfilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetFileEncryptionErrors(usePassword: true, keyfilePath)).Concat(GetEncryptionFilePathErrors(filePaths));
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
            yield return ErrorMessages.InvalidPrivateKeyFile;
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && !File.Exists(privateKeyPath))
        {
            yield return ErrorMessages.NonExistentPrivateKeyFile;
        }
    }

    public static bool FileDecryptionWithPassword(bool usePassword, string keyfilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionErrors(usePassword, keyfilePath).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileDecryptionErrors(bool usePassword, string keyfilePath)
    {
        if (!usePassword && string.IsNullOrEmpty(keyfilePath)) { yield return PasswordOrKeyfileError; }
        if (!string.IsNullOrEmpty(keyfilePath) && !File.Exists(keyfilePath)) { yield return "Please specify a keyfile that exists."; }
    }

    private static IEnumerable<string> GetDecryptionFilePathErrors(string[] filePaths)
    {
        if (filePaths == null)
        {
            yield return FileOrFolderError;
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

    public static bool FileDecryptionWithPublicKeyFile(string privateKeyPath, string[] publicKeyPaths, string keyfilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionWithPublicKeyFileErrors(privateKeyPath, publicKeyPaths).Concat(GetFileDecryptionErrors(usePassword: true, keyfilePath)).Concat(GetDecryptionFilePathErrors(filePaths));
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
            yield return ErrorMessages.PublicKey;
        }
        else if (publicKeyPaths.Length > 1)
        {
            yield return ErrorMessages.SinglePublicKey;
        }
        else if (!publicKeyPaths[0].EndsWith(Constants.PublicKeyExtension))
        {
            yield return ErrorMessages.InvalidPublicKeyFile;
        }
        else if (!File.Exists(publicKeyPaths[0]))
        {
            yield return ErrorMessages.NonExistentPublicKeyFile;
        }
    }

    public static bool FileDecryptionWithPublicKeyString(string privateKeyPath, string[] encodedPublicKeys, string keyfilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionWithPublicKeyStringErrors(privateKeyPath, encodedPublicKeys).Concat(GetFileDecryptionErrors(usePassword: true, keyfilePath)).Concat(GetDecryptionFilePathErrors(filePaths));
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
            yield return ErrorMessages.PublicKey;
        }
        else if (encodedPublicKeys.Length > 1)
        {
            yield return ErrorMessages.SinglePublicKey;
        }
        else if (encodedPublicKeys[0].Length != Constants.PublicKeyLength)
        {
            yield return ErrorMessages.InvalidPublicKey;
        }
    }

    public static bool FileDecryptionWithPrivateKey(string privateKeyPath, string keyfilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetFileDecryptionErrors(usePassword: true, keyfilePath)).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }
}