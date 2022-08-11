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
using Geralt;

namespace Kryptor;

public static class FileDecryption
{
    public static void DecryptEachFileWithPassword(string[] filePaths, byte[] passwordBytes)
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
                var keyEncryptionKey = GC.AllocateArray<byte>(Constants.EncryptionKeyLength, pinned: true);
                Argon2id.DeriveKey(keyEncryptionKey, passwordBytes, salt, Constants.Iterations, Constants.MemorySize);
                DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException) { DisplayMessage.FilePathError(inputFilePath, ex.Message); }
                else { DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile); }
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(passwordBytes);
        DisplayMessage.SuccessfullyDecrypted(space: false);
    }
    
    public static void DecryptEachFileWithSymmetricKey(string[] filePaths, byte[] symmetricKey)
    {
        if (filePaths == null || symmetricKey == null) { return; }
        foreach (string inputFilePath in filePaths)
        {
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                var keyEncryptionKey = GC.AllocateArray<byte>(Constants.EncryptionKeyLength, pinned: true);
                BLAKE2b.DeriveKey(keyEncryptionKey, symmetricKey, Constants.Personalisation, salt);
                DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException) { DisplayMessage.FilePathError(inputFilePath, ex.Message); }
                else { DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile); }
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(symmetricKey);
        DisplayMessage.SuccessfullyDecrypted(space: false);
    }

    public static void DecryptEachFileWithPublicKey(byte[] recipientPrivateKey, char[] password, byte[] senderPublicKey, byte[] presharedKey, string[] filePaths)
    {
        if (filePaths == null || recipientPrivateKey == null || senderPublicKey == null) { return; }
        recipientPrivateKey = PrivateKey.Decrypt(recipientPrivateKey, password);
        if (recipientPrivateKey == null) { return; }
        var sharedSecret = GC.AllocateArray<byte>(X25519.SharedSecretSize, pinned: true);
        X25519.DeriveRecipientSharedSecret(sharedSecret, recipientPrivateKey, senderPublicKey, presharedKey);
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                byte[] ephemeralPublicKey = FileHeaders.ReadEphemeralPublicKey(inputFile);
                var ephemeralSharedSecret = GC.AllocateArray<byte>(X25519.SharedSecretSize, pinned: true);
                X25519.DeriveRecipientSharedSecret(ephemeralSharedSecret, recipientPrivateKey, ephemeralPublicKey, presharedKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                var inputKeyingMaterial = GC.AllocateArray<byte>(ephemeralSharedSecret.Length + sharedSecret.Length, pinned: true);
                Spans.Concat(inputKeyingMaterial, ephemeralSharedSecret, sharedSecret);
                var keyEncryptionKey = GC.AllocateArray<byte>(Constants.EncryptionKeyLength, pinned: true);
                BLAKE2b.DeriveKey(keyEncryptionKey, inputKeyingMaterial, Constants.Personalisation, salt);
                CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                CryptographicOperations.ZeroMemory(inputKeyingMaterial);
                DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException) { DisplayMessage.FilePathError(inputFilePath, ex.Message); }
                else { DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile); }
            }
        }
        CryptographicOperations.ZeroMemory(recipientPrivateKey);
        CryptographicOperations.ZeroMemory(sharedSecret);
        CryptographicOperations.ZeroMemory(presharedKey);
        DisplayMessage.SuccessfullyDecrypted();
    }
    
    public static void DecryptEachFileWithPrivateKey(byte[] privateKey, char[] password, byte[] presharedKey, string[] filePaths)
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
                var ephemeralSharedSecret = GC.AllocateArray<byte>(X25519.SharedSecretSize, pinned: true);
                X25519.DeriveSenderSharedSecret(ephemeralSharedSecret, privateKey, ephemeralPublicKey, presharedKey);
                byte[] salt = FileHeaders.ReadSalt(inputFile);
                var keyEncryptionKey = GC.AllocateArray<byte>(Constants.EncryptionKeyLength, pinned: true);
                BLAKE2b.DeriveKey(keyEncryptionKey, ephemeralSharedSecret, Constants.Personalisation, salt);
                CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                DecryptInputFile(inputFile, ephemeralPublicKey, keyEncryptionKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException) { DisplayMessage.FilePathError(inputFilePath, ex.Message); }
                else { DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile); }
            }
        }
        CryptographicOperations.ZeroMemory(privateKey);
        CryptographicOperations.ZeroMemory(presharedKey);
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