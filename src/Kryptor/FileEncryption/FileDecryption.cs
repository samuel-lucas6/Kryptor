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

namespace Kryptor;

public static class FileDecryption
{
    public static unsafe void DecryptEachFileWithPassword(string[] filePaths, byte[] passwordBytes)
    {
        if (filePaths == null || passwordBytes == null) { return; }
        foreach (string inputFilePath in filePaths)
        {
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                DisplayMessage.DerivingKeyFromPassword();
                byte[] keyEncryptionKey = PasswordHash.ArgonHashBinary(passwordBytes, salt, Constants.Iterations, Constants.MemorySize, Constants.EncryptionKeyLength, PasswordHash.ArgonAlgorithm.Argon_2ID13);
                fixed (byte* ignored = keyEncryptionKey) { DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey); }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException)
                {
                    DisplayMessage.FilePathMessage(inputFilePath, ex.Message);
                    return;
                }
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile);
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(passwordBytes);
        DisplayMessage.SuccessfullyDecrypted(space: false);
    }

    public static unsafe void DecryptEachFileWithPublicKey(byte[] recipientPrivateKey, char[] password, byte[] senderPublicKey, string[] filePaths)
    {
        if (filePaths == null || recipientPrivateKey == null || senderPublicKey == null) { return; }
        recipientPrivateKey = PrivateKey.Decrypt(recipientPrivateKey, password);
        if (recipientPrivateKey == null) { return; }
        byte[] sharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, senderPublicKey);
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(recipientPrivateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                byte[] keyEncryptionKey = KeyDerivation.Blake2b(ephemeralSharedSecret, sharedSecret, salt);
                fixed (byte* ignored = keyEncryptionKey) { DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey); }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException)
                {
                    DisplayMessage.FilePathMessage(inputFilePath, ex.Message);
                    return;
                }
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile);
            }
        }
        CryptographicOperations.ZeroMemory(recipientPrivateKey);
        CryptographicOperations.ZeroMemory(sharedSecret);
        DisplayMessage.SuccessfullyDecrypted();
    }
    
    public static unsafe void DecryptEachFileWithPrivateKey(byte[] privateKey, char[] password, string[] filePaths)
    {
        if (filePaths == null || privateKey == null) { return; }
        privateKey = PrivateKey.Decrypt(privateKey, password);
        if (privateKey == null) { return; }
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] ephemeralSharedSecret = KeyExchange.GetSharedSecret(privateKey, ephemeralPublicKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                byte[] keyEncryptionKey = KeyDerivation.Blake2b(ephemeralSharedSecret, salt);
                fixed (byte* ignored = keyEncryptionKey) { DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey); }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException)
                {
                    DisplayMessage.FilePathMessage(inputFilePath, ex.Message);
                    return;
                }
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile);
            }
        }
        CryptographicOperations.ZeroMemory(privateKey);
        DisplayMessage.SuccessfullyDecrypted();
    }
    
    private static void DecryptInputFile(FileStream inputFile, byte[] ephemeralPublicKey, byte[] keyEncryptionKey)
    {
        string outputFilePath = FileHandling.GetDecryptedOutputFilePath(inputFile.Name);
        DisplayMessage.DecryptingFile(inputFile.Name, outputFilePath);
        DecryptFile.Decrypt(inputFile, outputFilePath, ephemeralPublicKey, keyEncryptionKey);
        CryptographicOperations.ZeroMemory(keyEncryptionKey);
    }
}