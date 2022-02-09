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
using System.Linq;
using System.Security.Cryptography;

namespace KryptorCLI;

public static class DirectoryDecryption
{
    public static void UsingPassword(string directoryPath, byte[] passwordBytes)
    {
        try
        {
            string[] filePaths = GetFiles(directoryPath);
            string saltFilePath = Path.Combine(directoryPath, Constants.SaltFileName);
            if (!File.Exists(saltFilePath)) { throw new FileNotFoundException("No salt file was found, so it's not possible to decrypt the directory. Please decrypt these files individually."); }
            byte[] salt = File.ReadAllBytes(saltFilePath);
            Globals.TotalCount--;
            if (salt.Length != Constants.SaltLength) { throw new ArgumentException("Invalid salt length."); }
            DisplayMessage.DerivingKeyFromPassword();
            byte[] keyEncryptionKey = KeyDerivation.Argon2id(passwordBytes, salt);
            DecryptEachFileWithPassword(filePaths, keyEncryptionKey);
            bool anyKryptorFiles = Directory.EnumerateFiles(directoryPath, searchPattern: $"*{Constants.EncryptedExtension}", SearchOption.AllDirectories).Any();
            if (!anyKryptorFiles) { FileHandling.DeleteFile(saltFilePath); }
            RestoreDirectoryNames.AllDirectories(directoryPath);
            DisplayMessage.DirectoryDecryptionComplete(directoryPath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DirectoryException(directoryPath, ex);
        }
    }
    
    private static string[] GetFiles(string directoryPath)
    {
        DisplayMessage.WriteLine($"Beginning decryption of \"{Path.GetFileName(directoryPath)}\" directory...", ConsoleColor.Blue);
        Console.WriteLine();
        string[] filePaths = FileHandling.GetAllFiles(directoryPath);
        // -1 for the specified directory
        Globals.TotalCount += filePaths.Length - 1;
        return filePaths;
    }

    private static void DecryptEachFileWithPassword(string[] filePaths, byte[] keyEncryptionKey)
    {
        foreach (string inputFilePath in filePaths)
        {
            bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
            if (!validFilePath) { continue; }
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                FileException(inputFilePath, ex);
            }
        }
        CryptographicOperations.ZeroMemory(keyEncryptionKey);
    }
    
    private static void DecryptInputFile(FileStream inputFile, byte[] ephemeralPublicKey, byte[] keyEncryptionKey)
    {
        string outputFilePath = FileHandling.GetDecryptedOutputFilePath(inputFile.Name);
        Console.WriteLine();
        DisplayMessage.DecryptingFile(inputFile.Name, outputFilePath);
        DecryptFile.Decrypt(inputFile, outputFilePath, ephemeralPublicKey, keyEncryptionKey);
    }

    public static void UsingPublicKey(string directoryPath, byte[] sharedSecret, byte[] recipientPrivateKey)
    {
        try
        {
            FilePathValidation.DirectoryDecryption(directoryPath);
            string[] filePaths = GetFiles(directoryPath);
            DecryptEachFileWithPublicKey(filePaths, sharedSecret, recipientPrivateKey);
            RestoreDirectoryNames.AllDirectories(directoryPath);
            DisplayMessage.DirectoryDecryptionComplete(directoryPath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DirectoryException(directoryPath, ex);
        }
    }

    private static void DecryptEachFileWithPublicKey(string[] filePaths, byte[] sharedSecret, byte[] recipientPrivateKey)
    {
        foreach (string inputFilePath in filePaths)
        {
            bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
            if (!validFilePath) { continue; }
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                byte[] keyEncryptionKey = KeyDerivation.Blake2(ephemeralSharedSecret, sharedSecret, salt);
                DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
                CryptographicOperations.ZeroMemory(keyEncryptionKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                FileException(inputFilePath, ex);
            }
        }
    }

    public static void UsingPrivateKey(string directoryPath, byte[] privateKey)
    {
        try
        {
            FilePathValidation.DirectoryDecryption(directoryPath);
            string[] filePaths = GetFiles(directoryPath);
            DecryptEachFileWithPrivateKey(filePaths, privateKey);
            RestoreDirectoryNames.AllDirectories(directoryPath);
            DisplayMessage.DirectoryDecryptionComplete(directoryPath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DirectoryException(directoryPath, ex);
        }
    }

    private static void DecryptEachFileWithPrivateKey(string[] filePaths, byte[] privateKey)
    {
        foreach (string inputFilePath in filePaths)
        {
            bool validFilePath = FilePathValidation.FileDecryption(inputFilePath);
            if (!validFilePath) { continue; }
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(privateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                byte[] keyEncryptionKey = KeyDerivation.Blake2(ephemeralSharedSecret, salt);
                DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
                CryptographicOperations.ZeroMemory(keyEncryptionKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                FileException(inputFilePath, ex);
            }
        }
    }

    private static void DirectoryException(string directoryPath, Exception ex)
    {
        if (ex is ArgumentException or FileNotFoundException)
        {
            DisplayMessage.FilePathError(directoryPath, ex.Message);
            return;
        }
        DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to decrypt the directory.");
    }

    private static void FileException(string inputFilePath, Exception ex)
    {
        if (ex is ArgumentException)
        {
            DisplayMessage.FilePathMessage(inputFilePath, ex.Message);
            return;
        }
        DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile);
    }
}