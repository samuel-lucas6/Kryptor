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
using System.Security.Cryptography;
using Sodium;

namespace KryptorCLI;

public static class DirectoryEncryption
{
    public static void UsingPassword(string directoryPath, byte[] passwordBytes)
    {
        bool overwriteOption = Globals.Overwrite;
        try
        {
            FilePathValidation.DirectoryEncryption(directoryPath);
            string backupDirectoryPath = BackupDirectory(directoryPath);
            string[] filePaths = GetFiles(directoryPath, out string newDirectoryPath);
            byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
            CreateSaltFile(newDirectoryPath, salt);
            byte[] keyEncryptionKey = KeyDerivation.Argon2id(passwordBytes, salt);
            EncryptEachFileWithPassword(filePaths, salt, keyEncryptionKey);
            RenameBackupDirectory(backupDirectoryPath, directoryPath);
            DisplayMessage.DirectoryEncryptionComplete(directoryPath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DirectoryException(directoryPath, ex);
        }
        finally
        {
            Globals.Overwrite = overwriteOption;
        }
    }

    private static string BackupDirectory(string directoryPath)
    {
        if (Globals.Overwrite) { return null; }
        string destinationDirectoryPath = FileHandling.GetUniqueDirectoryPath($"{directoryPath} - Copy");
        Console.WriteLine($"Copying \"{Path.GetFileName(directoryPath)}\" directory => \"{Path.GetFileName(destinationDirectoryPath)}\" because you didn't specify -o|--overwrite...");
        FileHandling.CopyDirectory(directoryPath, destinationDirectoryPath, copySubdirectories: true);
        Console.WriteLine();
        Globals.Overwrite = true;
        return destinationDirectoryPath;
    }

    private static string[] GetFiles(string directoryPath, out string newDirectoryPath)
    {
        newDirectoryPath = directoryPath;
        try
        {
            if (Globals.EncryptFileNames)
            {
                DisplayMessage.WriteLine($"Renaming \"{Path.GetFileName(directoryPath)}\" and subdirectories...", ConsoleColor.Blue);
                newDirectoryPath = ObfuscateDirectoryNames.AllDirectories(directoryPath);
            }
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to obfuscate the directory names.");
        }
        DisplayMessage.WriteLine($"Beginning encryption of \"{Path.GetFileName(newDirectoryPath)}\" directory...", ConsoleColor.Blue);
        Console.WriteLine();
        string[] filePaths = FileHandling.GetAllFiles(newDirectoryPath);
        // -1 for the specified directory
        Globals.TotalCount += filePaths.Length - 1;
        return filePaths;
    }

    private static void CreateSaltFile(string directoryPath, byte[] salt)
    {
        string saltFilePath = Path.Combine(directoryPath, Constants.SaltFileName);
        File.WriteAllBytes(saltFilePath, salt);
        FileHandling.SetFileAttributesReadOnly(saltFilePath);
    }

    private static void EncryptEachFileWithPassword(string[] filePaths, byte[] salt, byte[] keyEncryptionKey)
    {
        foreach (string inputFilePath in filePaths)
        {
            if (!FilePathValidation.FileEncryption(inputFilePath)) { continue; }
            // Fill unused file header with random public key
            using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
            EncryptInputFile(inputFilePath, ephemeralKeyPair.PublicKey, salt, keyEncryptionKey);
        }
        CryptographicOperations.ZeroMemory(keyEncryptionKey);
    }

    private static void EncryptInputFile(string inputFilePath, byte[] ephemeralPublicKey, byte[] salt, byte[] keyEncryptionKey)
    {
        try
        {
            string outputFilePath = FileHandling.GetEncryptedOutputFilePath(inputFilePath);
            Console.WriteLine();
            DisplayMessage.EncryptingFile(inputFilePath, outputFilePath);
            EncryptFile.Encrypt(inputFilePath, outputFilePath, ephemeralPublicKey, salt, keyEncryptionKey);
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
        }
    }

    private static void RenameBackupDirectory(string backupDirectoryPath, string originalDirectoryPath)
    {
        if (string.IsNullOrEmpty(backupDirectoryPath) || Directory.Exists(originalDirectoryPath)) { return; }
        Console.WriteLine($"Renaming \"{Path.GetFileName(backupDirectoryPath)}\" backup directory => \"{Path.GetFileName(originalDirectoryPath)}\"...");
        Directory.Move(backupDirectoryPath, originalDirectoryPath);
    }

    public static void UsingPublicKey(string directoryPath, byte[] sharedSecret, byte[] recipientPublicKey)
    {
        bool overwriteOption = Globals.Overwrite;
        try
        {
            FilePathValidation.DirectoryEncryption(directoryPath);
            string backupDirectoryPath = BackupDirectory(directoryPath);
            string[] filePaths = GetFiles(directoryPath, newDirectoryPath: out _);
            EncryptEachFileWithPublicKey(filePaths, sharedSecret, recipientPublicKey);
            RenameBackupDirectory(backupDirectoryPath, directoryPath);
            DisplayMessage.DirectoryEncryptionComplete(directoryPath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DirectoryException(directoryPath, ex);
        }
        finally
        {
            Globals.Overwrite = overwriteOption;
        }
    }

    private static void EncryptEachFileWithPublicKey(string[] filePaths, byte[] sharedSecret, byte[] recipientPublicKey)
    {
        foreach (string inputFilePath in filePaths)
        {
            if (!FilePathValidation.FileEncryption(inputFilePath)) { continue; }
            byte[] ephemeralSharedSecret = KeyExchange.GetPublicKeySharedSecret(recipientPublicKey, out byte[] ephemeralPublicKey);
            byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
            byte[] keyEncryptionKey = KeyDerivation.Blake2(sharedSecret, ephemeralSharedSecret, salt);
            EncryptInputFile(inputFilePath, ephemeralPublicKey, salt, keyEncryptionKey);
            CryptographicOperations.ZeroMemory(keyEncryptionKey);
        }
    }

    public static void UsingPrivateKey(string directoryPath, byte[] privateKey)
    {
        bool overwriteOption = Globals.Overwrite;
        try
        {
            FilePathValidation.DirectoryEncryption(directoryPath);
            string backupDirectoryPath = BackupDirectory(directoryPath);
            string[] filePaths = GetFiles(directoryPath, newDirectoryPath: out _);
            EncryptEachFileWithPrivateKey(filePaths, privateKey);
            RenameBackupDirectory(backupDirectoryPath, directoryPath);
            DisplayMessage.DirectoryEncryptionComplete(directoryPath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DirectoryException(directoryPath, ex);
        }
        finally
        {
            Globals.Overwrite = overwriteOption;
        }
    }

    private static void EncryptEachFileWithPrivateKey(string[] filePaths, byte[] privateKey)
    {
        foreach (string inputFilePath in filePaths)
        {
            if (!FilePathValidation.FileEncryption(inputFilePath)) { continue; }
            byte[] ephemeralSharedSecret = KeyExchange.GetPrivateKeySharedSecret(privateKey, out byte[] ephemeralPublicKey);
            byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
            byte[] keyEncryptionKey = KeyDerivation.Blake2(ephemeralSharedSecret, salt);
            EncryptInputFile(inputFilePath, ephemeralPublicKey, salt, keyEncryptionKey);
            CryptographicOperations.ZeroMemory(keyEncryptionKey);
        }
    }

    private static void DirectoryException(string directoryPath, Exception ex)
    {
        if (ex is ArgumentException)
        {
            DisplayMessage.FilePathError(directoryPath, ex.Message);
            return;
        }
        DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to encrypt the directory.");
    }
}