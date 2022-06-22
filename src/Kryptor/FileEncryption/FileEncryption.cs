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
using System.Security.Cryptography;
using Sodium;

namespace Kryptor;

public static class FileEncryption
{
    public static unsafe void EncryptEachFileWithPassword(string[] filePaths, byte[] passwordBytes)
    {
        if (filePaths == null || passwordBytes == null) { return; }
        foreach (string inputFilePath in filePaths)
        {
            try
            {
                bool directory = FileHandling.IsDirectory(inputFilePath);
                string zipFilePath = inputFilePath + Constants.ZipFileExtension;
                if (directory) { FileHandling.CreateZipFile(inputFilePath, zipFilePath); }
                byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                DisplayMessage.DerivingKeyFromPassword();
                byte[] keyEncryptionKey = PasswordHash.ArgonHashBinary(passwordBytes, salt, Constants.Iterations, Constants.MemorySize, Constants.EncryptionKeyLength, PasswordHash.ArgonAlgorithm.Argon_2ID13);
                fixed (byte* ignored = keyEncryptionKey)
                {
                    // Fill unused header with random public key
                    using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
                    EncryptInputFile(directory ? zipFilePath : inputFilePath, directory, ephemeralKeyPair.PublicKey, salt, keyEncryptionKey);
                }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(passwordBytes);
        DisplayMessage.SuccessfullyEncrypted(space: false);
    }
    
    public static unsafe void EncryptEachFileWithPublicKey(byte[] senderPrivateKey, char[] password, byte[] recipientPublicKey, string[] filePaths)
    {
        if (filePaths == null || senderPrivateKey == null || recipientPublicKey == null) { return; }
        senderPrivateKey = PrivateKey.Decrypt(senderPrivateKey, password);
        if (senderPrivateKey == null) { return; }
        byte[] sharedSecret = KeyExchange.GetSharedSecret(senderPrivateKey, recipientPublicKey);
        CryptographicOperations.ZeroMemory(senderPrivateKey);
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            try
            {
                bool directory = FileHandling.IsDirectory(inputFilePath);
                string zipFilePath = inputFilePath + Constants.ZipFileExtension;
                if (directory) { FileHandling.CreateZipFile(inputFilePath, zipFilePath); }
                byte[] ephemeralSharedSecret = KeyExchange.GetPublicKeySharedSecret(recipientPublicKey, out byte[] ephemeralPublicKey);
                byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                byte[] keyEncryptionKey = KeyDerivation.Blake2b(ephemeralSharedSecret, sharedSecret, salt);
                fixed (byte* ignored = keyEncryptionKey) { EncryptInputFile(directory ? zipFilePath : inputFilePath, directory, ephemeralPublicKey, salt, keyEncryptionKey); }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
            }
        }
        CryptographicOperations.ZeroMemory(sharedSecret);
        DisplayMessage.SuccessfullyEncrypted();
    }

    public static unsafe void EncryptEachFileWithPrivateKey(byte[] privateKey, char[] password, string[] filePaths)
    {
        if (filePaths == null || privateKey == null) { return; }
        privateKey = PrivateKey.Decrypt(privateKey, password);
        if (privateKey == null) { return; }
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            try
            {
                bool directory = FileHandling.IsDirectory(inputFilePath);
                string zipFilePath = inputFilePath + Constants.ZipFileExtension;
                if (directory) { FileHandling.CreateZipFile(inputFilePath, zipFilePath); }
                byte[] ephemeralSharedSecret = KeyExchange.GetPrivateKeySharedSecret(privateKey, out byte[] ephemeralPublicKey);
                byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                byte[] keyEncryptionKey = KeyDerivation.Blake2b(ephemeralSharedSecret, salt);
                fixed (byte* ignored = keyEncryptionKey) { EncryptInputFile(directory ? zipFilePath : inputFilePath, directory, ephemeralPublicKey, salt, keyEncryptionKey); }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
            }
        }
        CryptographicOperations.ZeroMemory(privateKey);
        DisplayMessage.SuccessfullyEncrypted();
    }
    
    private static void EncryptInputFile(string inputFilePath, bool directory, byte[] ephemeralPublicKey, byte[] salt, byte[] keyEncryptionKey)
    {
        string outputFilePath = FileHandling.GetEncryptedOutputFilePath(inputFilePath);
        DisplayMessage.EncryptingFile(inputFilePath, outputFilePath);
        EncryptFile.Encrypt(inputFilePath, outputFilePath, directory, ephemeralPublicKey, salt, keyEncryptionKey);
        CryptographicOperations.ZeroMemory(keyEncryptionKey);
    }
}