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
using System.Security.Cryptography;

namespace KryptorCLI;

public static class FileDecryption
{
    public static void DecryptEachFileWithPassword(string[] filePaths, byte[] passwordBytes)
    {
        if (filePaths == null || passwordBytes == null) { return; }
        foreach (string inputFilePath in filePaths)
        {
            UsingPassword(inputFilePath, passwordBytes);
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(passwordBytes);
        DisplayMessage.SuccessfullyDecrypted(space: false);
    }

    private static void UsingPassword(string inputFilePath, byte[] passwordBytes)
    {
        try
        {
            if (FileHandling.IsDirectory(inputFilePath))
            {
                DirectoryDecryption.UsingPassword(inputFilePath, passwordBytes);
                return;
            }
            using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
            byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
            byte[] salt = FileHeaders.ReadSalt(inputFile);
            byte[] keyEncryptionKey = KeyDerivation.Argon2id(passwordBytes, salt);
            DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            FileException(inputFilePath, ex);
        }
    }
    
    private static void DecryptInputFile(FileStream inputFile, byte[] ephemeralPublicKey, byte[] keyEncryptionKey)
    {
        string outputFilePath = FileHandling.GetDecryptedOutputFilePath(inputFile.Name);
        DisplayMessage.DecryptingFile(inputFile.Name, outputFilePath);
        DecryptFile.Decrypt(inputFile, outputFilePath, ephemeralPublicKey, keyEncryptionKey);
        CryptographicOperations.ZeroMemory(keyEncryptionKey);
    }

    public static void DecryptEachFileWithPublicKey(byte[] recipientPrivateKey, char[] password, byte[] senderPublicKey, string[] filePaths)
    {
        if (filePaths == null || recipientPrivateKey == null || senderPublicKey == null) { return; }
        recipientPrivateKey = PrivateKey.Decrypt(recipientPrivateKey, password);
        if (recipientPrivateKey == null) { return; }
        byte[] sharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, senderPublicKey);
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            UsingPublicKey(inputFilePath, sharedSecret, recipientPrivateKey);
        }
        CryptographicOperations.ZeroMemory(recipientPrivateKey);
        CryptographicOperations.ZeroMemory(sharedSecret);
        DisplayMessage.SuccessfullyDecrypted();
    }

    private static void UsingPublicKey(string inputFilePath, byte[] sharedSecret, byte[] recipientPrivateKey)
    {
        try
        {
            if (FileHandling.IsDirectory(inputFilePath))
            {
                DirectoryDecryption.UsingPublicKey(inputFilePath, sharedSecret, recipientPrivateKey);
                return;
            }
            using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
            byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
            byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, ephemeralPublicKey);
            byte[] salt = FileHeaders.ReadSalt(inputFile);
            byte[] keyEncryptionKey = KeyDerivation.Blake2(sharedSecret, ephemeralSharedSecret, salt);
            DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            FileException(inputFilePath, ex);
        }
    }

    public static void DecryptEachFileWithPrivateKey(byte[] privateKey, char[] password, string[] filePaths)
    {
        if (filePaths == null || privateKey == null) { return; }
        privateKey = PrivateKey.Decrypt(privateKey, password);
        if (privateKey == null) { return; }
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            UsingPrivateKey(inputFilePath, privateKey);
        }
        CryptographicOperations.ZeroMemory(privateKey);
        DisplayMessage.SuccessfullyDecrypted();
    }

    private static void UsingPrivateKey(string inputFilePath, byte[] privateKey)
    {
        try
        {
            if (FileHandling.IsDirectory(inputFilePath))
            {
                DirectoryDecryption.UsingPrivateKey(inputFilePath, privateKey);
                return;
            }
            using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
            byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
            byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(privateKey, ephemeralPublicKey);
            byte[] salt = FileHeaders.ReadSalt(inputFile);
            byte[] keyEncryptionKey = KeyDerivation.Blake2(ephemeralSharedSecret, salt);
            DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            FileException(inputFilePath, ex);
        }
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