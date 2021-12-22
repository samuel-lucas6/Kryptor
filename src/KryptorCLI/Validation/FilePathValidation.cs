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

using System;
using System.IO;
using System.Collections.Generic;

namespace KryptorCLI;

public static class FilePathValidation
{
    private const string FileDoesNotExist = "This file doesn't exist.";
    private const string FileOrFolderDoesNotExist = "This file/folder doesn't exist.";
    private const string FileInaccessible = "Unable to access the file.";
    private const string DirectoryEmpty = "This directory is empty.";

    public static bool FileEncryption(string inputFilePath)
    {
        string errorMessage = GetFileEncryptionError(inputFilePath);
        if (string.IsNullOrEmpty(errorMessage)) { return true; }
        DisplayMessage.FilePathError(inputFilePath, errorMessage);
        return false;
    }

    public static string GetFileEncryptionError(string inputFilePath)
    {
        if (Directory.Exists(inputFilePath))
        {
            return FileHandling.IsDirectoryEmpty(inputFilePath) ? DirectoryEmpty : null;
        }
        if (!File.Exists(inputFilePath)) { return FileOrFolderDoesNotExist; }
        bool? validMagicBytes = FileHandling.IsKryptorFile(inputFilePath);
        if (validMagicBytes == null) { return FileInaccessible; }
        if (FileHandling.HasKryptorExtension(inputFilePath) || validMagicBytes == true)
        {
            return "This file has already been encrypted.";
        }
        return null;
    }

    public static string KeyfilePath(string keyfilePath)
    {
        try
        {
            if (string.IsNullOrEmpty(keyfilePath) || File.Exists(keyfilePath)) { return keyfilePath; }
            if (Directory.Exists(keyfilePath))
            {
                keyfilePath = Path.Combine(keyfilePath, ObfuscateFileName.GetRandomFileName());
            }
            if (!keyfilePath.EndsWith(Constants.KeyfileExtension)) { keyfilePath += Constants.KeyfileExtension; }
            if (File.Exists(keyfilePath)) { return keyfilePath; }
            Keyfiles.GenerateKeyfile(keyfilePath);
            Console.WriteLine($"Randomly generated keyfile: {Path.GetFileName(keyfilePath)}");
            Console.WriteLine();
            return keyfilePath;
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(keyfilePath, ex.GetType().Name, "Unable to randomly generate keyfile.");
            return null;
        }
    }

    public static void DirectoryEncryption(string directoryPath)
    {
        string saltFilePath = Path.Combine(directoryPath, Constants.SaltFileName);
        if (File.Exists(saltFilePath)) { throw new ArgumentException("This directory has already been encrypted."); }
    }

    public static void DirectoryDecryption(string directoryPath)
    {
        string saltFilePath = Path.Combine(directoryPath, Constants.SaltFileName);
        if (File.Exists(saltFilePath)) { throw new ArgumentException("This directory was encrypted using a password."); }
    }

    public static bool FileDecryption(string inputFilePath)
    {
        if (inputFilePath.Contains(Constants.SaltFileName))
        {
            --Globals.TotalCount;
            return false;
        }
        string errorMessage = GetFileDecryptionError(inputFilePath);
        if (string.IsNullOrEmpty(errorMessage)) { return true; }
        DisplayMessage.FilePathError(inputFilePath, errorMessage);
        return false;
    }

    public static string GetFileDecryptionError(string inputFilePath)
    {
        if (Directory.Exists(inputFilePath))
        {
            return FileHandling.IsDirectoryEmpty(inputFilePath) ? DirectoryEmpty : null;
        }
        if (!File.Exists(inputFilePath)) { return FileOrFolderDoesNotExist; }
        bool? validMagicBytes = FileHandling.IsKryptorFile(inputFilePath);
        if (validMagicBytes == null) { return FileInaccessible; }
        if (!FileHandling.HasKryptorExtension(inputFilePath) || validMagicBytes == false)
        {
            return "This file hasn't been encrypted.";
        }
        return null;
    }

    public static bool GenerateKeyPair(string directoryPath, int keyPairType)
    {
        IEnumerable<string> errorMessages = GetGenerateKeyPairError(directoryPath, keyPairType);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetGenerateKeyPairError(string directoryPath, int keyPairType)
    {
        if (keyPairType is < 1 or > 2)
        {
            yield return "Please enter a valid number.";
        }
        bool defaultKeyDirectory = string.Equals(directoryPath, Constants.DefaultKeyDirectory);
        if (!defaultKeyDirectory && !Directory.Exists(directoryPath))
        {
            yield return "This directory doesn't exist.";
        }
        else if (defaultKeyDirectory && !Globals.Overwrite)
        {
            if (keyPairType == 1 && (File.Exists(Constants.DefaultEncryptionPublicKeyPath) || File.Exists(Constants.DefaultEncryptionPrivateKeyPath)))
            {
                yield return "An encryption key pair already exists. Please specify -o|--overwrite if you want to overwrite your key pair.";
            }
            else if (keyPairType == 2 && (File.Exists(Constants.DefaultSigningPublicKeyPath) || File.Exists(Constants.DefaultSigningPrivateKeyPath)))
            {   
                yield return "A signing key pair already exists. Please specify -o|--overwrite if you want to overwrite your key pair.";
            }
        }
        else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 1)
        {
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PublicKeyExtension);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PrivateKeyExtension);
            if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath))
            {
                yield return "An encryption key pair already exists in the specified directory. Please specify -o|--overwrite if you want to overwrite your key pair.";
            }
        }
        else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 2)
        {
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PublicKeyExtension);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PrivateKeyExtension);
            if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath))
            {
                yield return "A signing key pair already exists in the specified directory. Please specify -o|--overwrite if you want to overwrite your key pair.";
            }
        }
    }

    public static bool RecoverPublicKey(string privateKeyPath)
    {
        IEnumerable<string> errorMessages = GetRecoverPublicKeyError(privateKeyPath);
        return DisplayMessage.AnyErrors(errorMessages);
    }

    private static IEnumerable<string> GetRecoverPublicKeyError(string privateKeyPath)
    {
        if (privateKeyPath == null)
        {
            yield return "Please specify a private key using [-x=filepath].";
        }
        else if (!File.Exists(privateKeyPath) || !privateKeyPath.EndsWith(Constants.PrivateKeyExtension))
        {
            yield return ErrorMessages.InvalidPrivateKeyFile;
        }
    }

    public static string GetFileSigningError(string inputFilePath)
    {
        if (Directory.Exists(inputFilePath)) { return ErrorMessages.NoFileToSign; }
        return !File.Exists(inputFilePath) ? FileDoesNotExist : null;
    }

    public static string GetSignatureVerifyError(string inputFilePath)
    {
        if (Directory.Exists(inputFilePath)) { return ErrorMessages.NoFileToVerify; }
        return !File.Exists(inputFilePath) ? "This file doesn't exist." : null;
    }

    public static string GetSignatureFilePath(string signatureFilePath, string[] filePaths)
    {
        if (!string.IsNullOrEmpty(signatureFilePath) || filePaths == null) { return signatureFilePath; }
        string possibleSignatureFilePath = filePaths[0] + Constants.SignatureExtension;
        return File.Exists(possibleSignatureFilePath) ? possibleSignatureFilePath : signatureFilePath;
    }
}