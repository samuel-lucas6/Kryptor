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

public static class FilePathValidation
{
    private static readonly char[] IllegalFileNameChars = {
        '\"', '<', '>', '|', '\0',
        (char) 1, (char) 2, (char) 3, (char) 4, (char) 5, (char) 6, (char) 7, (char) 8, (char) 9, (char) 10,
        (char) 11, (char) 12, (char) 13, (char) 14, (char) 15, (char) 16, (char) 17, (char) 18, (char) 19, (char) 20,
        (char) 21, (char) 22, (char) 23, (char) 24, (char) 25, (char) 26, (char) 27, (char) 28, (char) 29, (char) 30,
        (char) 31, ':', '*', '?', '\\', '/'
    };


    public static string GetFileEncryptionError(string inputFilePath)
    {
        if (Path.GetFileName(Path.TrimEndingDirectorySeparator(inputFilePath)).IndexOfAny(IllegalFileNameChars) != -1) { return "This file/directory name contains illegal characters for Windows, Linux, and/or macOS.";}
        if (Directory.Exists(inputFilePath)) { return FileHandling.IsDirectoryEmpty(inputFilePath) ? ErrorMessages.DirectoryEmpty : null; }
        return !File.Exists(inputFilePath) ? ErrorMessages.FileOrDirectoryDoesNotExist : null;
    }

    public static string GetFileDecryptionError(string inputFilePath)
    {
        if (Directory.Exists(inputFilePath)) { return FileHandling.IsDirectoryEmpty(inputFilePath) ? ErrorMessages.DirectoryEmpty : null; }
        return !File.Exists(inputFilePath) ? ErrorMessages.FileOrDirectoryDoesNotExist : null;
    }

    public static void GenerateKeyPair(string directoryPath, int keyPairType, bool encryption, bool signing)
    {
        IEnumerable<string> errorMessages = GetGenerateKeyPairErrors(directoryPath, keyPairType, encryption, signing);
        DisplayMessage.AllErrors(errorMessages);
    }

    private static IEnumerable<string> GetGenerateKeyPairErrors(string directoryPath, int keyPairType, bool encryption, bool signing)
    {
        if (keyPairType != 1 && keyPairType != 2) {
            yield return "Please enter a valid number.";
        }
        bool defaultKeyDirectory = string.Equals(directoryPath, Constants.DefaultKeyDirectory);
        if (!defaultKeyDirectory && !Directory.Exists(directoryPath)) {
            yield return ErrorMessages.GetFilePathError(directoryPath, "This directory doesn't exist.");
        }
        if (encryption && signing) {
            yield return "Please specify only one type of key pair to generate.";
        }
        else if (defaultKeyDirectory && !Globals.Overwrite) {
            if (keyPairType == 1 && (File.Exists(Constants.DefaultEncryptionPublicKeyPath) || File.Exists(Constants.DefaultEncryptionPrivateKeyPath))) {
                yield return "An encryption key pair already exists in the default directory. Please specify -o|--overwrite if you want to overwrite your key pair.";
            }
            else if (keyPairType == 2 && (File.Exists(Constants.DefaultSigningPublicKeyPath) || File.Exists(Constants.DefaultSigningPrivateKeyPath))) {   
                yield return "A signing key pair already exists in the default directory. Please specify -o|--overwrite if you want to overwrite your key pair.";
            }
        }
        else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 1) {
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PublicKeyExtension);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PrivateKeyExtension);
            if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath)) {
                yield return ErrorMessages.GetFilePathError(directoryPath, "An encryption key pair already exists in this directory. Please specify -o|--overwrite if you want to overwrite your key pair.");
            }
        }
        else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 2) {
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PublicKeyExtension);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PrivateKeyExtension);
            if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath)) {
                yield return ErrorMessages.GetFilePathError(directoryPath, "A signing key pair already exists in this directory. Please specify -o|--overwrite if you want to overwrite your key pair.");
            }
        }
    }

    public static void RecoverPublicKey(string privateKeyPath)
    {
        IEnumerable<string> errorMessages = GetRecoverPublicKeyErrors(privateKeyPath);
        DisplayMessage.AllErrors(errorMessages);
    }

    private static IEnumerable<string> GetRecoverPublicKeyErrors(string privateKeyPath)
    {
        if (string.IsNullOrEmpty(privateKeyPath)) {
            yield return "Please specify a private key file using -x:file.";
        }
        else if (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.InvalidPrivateKeyFile);
        }
        else if (!File.Exists(privateKeyPath)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.NonExistentPrivateKeyFile);
        }
    }
}