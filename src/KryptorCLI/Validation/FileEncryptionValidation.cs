/*
    Kryptor: A simple, modern, and secure encryption tool.
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

namespace KryptorCLI;

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
        if (!usePassword && string.IsNullOrEmpty(keyfilePath))
        {
            yield return PasswordOrKeyfileError;
        }
        if (Path.EndsInDirectorySeparator(keyfilePath) && !Directory.Exists(keyfilePath))
        {
            yield return "Please specify a valid keyfile directory.";
        }
        else if (File.Exists(keyfilePath) && FileHandling.GetFileLength(keyfilePath) < Constants.KeyfileLength)
        {
            yield return "Please specify a keyfile that is at least 64 bytes in size.";
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

    public static bool FileEncryptionWithPublicKey(string privateKeyPath, string publicKeyPath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, publicKeyPath).Concat(GetEncryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath, string publicKeyPath)
    {
        if (string.IsNullOrEmpty(privateKeyPath) && !File.Exists(Constants.DefaultEncryptionPrivateKeyPath))
        {
            yield return ErrorMessages.InvalidPrivateKeyFile;
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension) || !File.Exists(privateKeyPath)))
        {
            yield return ErrorMessages.InvalidPrivateKeyFile;
        }
        if (string.IsNullOrEmpty(publicKeyPath) || !publicKeyPath.EndsWith(Constants.PublicKeyExtension) || !File.Exists(publicKeyPath))
        {
            yield return ErrorMessages.InvalidPublicKey;
        }
    }

    public static bool FileEncryptionWithPublicKey(string privateKeyPath, char[] encodedPublicKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, encodedPublicKey).Concat(GetEncryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath, char[] encodedPublicKey)
    {
        if (string.IsNullOrEmpty(privateKeyPath) && !File.Exists(Constants.DefaultEncryptionPrivateKeyPath))
        {
            yield return ErrorMessages.InvalidPrivateKeyFile;
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension) || !File.Exists(privateKeyPath)))
        {
            yield return ErrorMessages.InvalidPrivateKeyFile;
        }
        if (encodedPublicKey.Length != Constants.PublicKeyLength)
        {
            yield return ErrorMessages.InvalidPublicKey;
        }
    }

    public static bool FileEncryptionWithPrivateKey(string privateKeyPath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetEncryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileEncryptionErrors(string privateKeyPath)
    {
        if (string.IsNullOrEmpty(privateKeyPath) && !File.Exists(Constants.DefaultEncryptionPrivateKeyPath))
        {
            yield return ErrorMessages.InvalidPrivateKeyFile;
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension) || !File.Exists(privateKeyPath)))
        {
            yield return ErrorMessages.InvalidPrivateKeyFile;
        }
    }

    public static bool FileDecryptionWithPassword(bool usePassword, string keyfilePath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileDecryptionErrors(usePassword, keyfilePath).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetFileDecryptionErrors(bool usePassword, string keyfilePath)
    {
        if (!usePassword && string.IsNullOrEmpty(keyfilePath))
        {
            yield return PasswordOrKeyfileError;
        }
        if (!string.IsNullOrEmpty(keyfilePath) && !File.Exists(keyfilePath))
        {
            yield return "Please specify a keyfile that exists.";
        }
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

    public static bool FileDecryptionWithPublicKey(string privateKeyPath, string publicKeyPath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, publicKeyPath).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    public static bool FileDecryptionWithPublicKey(string privateKeyPath, char[] encodedPublicKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath, encodedPublicKey).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }

    public static bool FileDecryptionWithPrivateKey(string privateKeyPath, string[] filePaths)
    {
        IEnumerable<string> errorMessages = GetFileEncryptionErrors(privateKeyPath).Concat(GetDecryptionFilePathErrors(filePaths));
        return DisplayMessage.AnyErrors(errorMessages);
    }
}