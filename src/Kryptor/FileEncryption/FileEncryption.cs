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
using System.Collections.Generic;
using System.Security.Cryptography;
using Geralt;
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
                bool directory = IsDirectory(inputFilePath, out string zipFilePath);
                byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                // Fill unused header with random public key
                using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
                DisplayMessage.DerivingKeyFromPassword();
                byte[] keyEncryptionKey = PasswordHash.ArgonHashBinary(passwordBytes, salt, Constants.Iterations, Constants.MemorySize, Constants.EncryptionKeyLength, PasswordHash.ArgonAlgorithm.Argon_2ID13);
                fixed (byte* ignored = keyEncryptionKey) { EncryptInputFile(directory ? zipFilePath : inputFilePath, directory, ephemeralKeyPair.PublicKey, salt, keyEncryptionKey); }
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
    
    public static unsafe void EncryptEachFileWithSymmetricKey(string[] filePaths, byte[] symmetricKey)
    {
        if (filePaths == null || symmetricKey == null) { return; }
        foreach (string inputFilePath in filePaths)
        {
            try
            {
                bool directory = IsDirectory(inputFilePath, out string zipFilePath);
                byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                // Fill unused header with random public key
                using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
                byte[] keyEncryptionKey = GenericHash.HashSaltPersonal(message: Array.Empty<byte>(), symmetricKey, salt, Constants.Personalisation, Constants.EncryptionKeyLength);
                fixed (byte* ignored = keyEncryptionKey) { EncryptInputFile(directory ? zipFilePath : inputFilePath, directory, ephemeralKeyPair.PublicKey, salt, keyEncryptionKey); }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(symmetricKey);
        DisplayMessage.SuccessfullyEncrypted(space: false);
    }
    
    public static unsafe void EncryptEachFileWithPublicKey(byte[] senderPrivateKey, char[] password, List<byte[]> recipientPublicKeys, byte[] presharedKey, string[] filePaths)
    {
        if (filePaths == null || senderPrivateKey == null || recipientPublicKeys == null) { return; }
        senderPrivateKey = PrivateKey.Decrypt(senderPrivateKey, password);
        if (senderPrivateKey == null) { return; }
        Globals.TotalCount *= recipientPublicKeys.Count;
        bool overwrite = Globals.Overwrite;
        Globals.Overwrite = false;
        int i = 0;
        foreach (byte[] recipientPublicKey in recipientPublicKeys)
        {
            if (i++ == recipientPublicKeys.Count - 1) { Globals.Overwrite = overwrite; }
            var sharedSecret = new byte[X25519.SharedSecretSize];
            X25519.DeriveSenderSharedSecret(sharedSecret, senderPrivateKey, recipientPublicKey, presharedKey);
            foreach (string inputFilePath in filePaths)
            {
                Console.WriteLine();
                try
                {
                    bool directory = IsDirectory(inputFilePath, out string zipFilePath);
                    var ephemeralPublicKey = new byte[X25519.PublicKeySize];
                    var ephemeralPrivateKey = new byte[X25519.PrivateKeySize];
                    X25519.GenerateKeyPair(ephemeralPublicKey, ephemeralPrivateKey);
                    var ephemeralSharedSecret = new byte[X25519.SharedSecretSize];
                    X25519.DeriveSenderSharedSecret(ephemeralSharedSecret, ephemeralPrivateKey, recipientPublicKey, presharedKey);
                    CryptographicOperations.ZeroMemory(ephemeralPrivateKey);
                    byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                    byte[] inputKeyingMaterial = Arrays.Concat(ephemeralSharedSecret, sharedSecret);
                    var keyEncryptionKey = new byte[Constants.EncryptionKeyLength];
                    BLAKE2b.DeriveKey(keyEncryptionKey, inputKeyingMaterial, Constants.Personalisation, salt);
                    CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                    CryptographicOperations.ZeroMemory(inputKeyingMaterial);
                    fixed (byte* ignored = keyEncryptionKey) { EncryptInputFile(directory ? zipFilePath : inputFilePath, directory, ephemeralPublicKey, salt, keyEncryptionKey); }
                }
                catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
                {
                    DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
                }
            }
            CryptographicOperations.ZeroMemory(sharedSecret);
        }
        CryptographicOperations.ZeroMemory(senderPrivateKey);
        CryptographicOperations.ZeroMemory(presharedKey);
        DisplayMessage.SuccessfullyEncrypted();
    }

    public static unsafe void EncryptEachFileWithPrivateKey(byte[] privateKey, char[] password, byte[] presharedKey, string[] filePaths)
    {
        if (filePaths == null || privateKey == null) { return; }
        privateKey = PrivateKey.Decrypt(privateKey, password);
        if (privateKey == null) { return; }
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            try
            {
                bool directory = IsDirectory(inputFilePath, out string zipFilePath);
                var ephemeralPublicKey = new byte[X25519.PublicKeySize];
                var ephemeralPrivateKey = new byte[X25519.PrivateKeySize];
                X25519.GenerateKeyPair(ephemeralPublicKey, ephemeralPrivateKey);
                CryptographicOperations.ZeroMemory(ephemeralPrivateKey);
                var ephemeralSharedSecret = new byte[X25519.SharedSecretSize];
                X25519.DeriveSenderSharedSecret(ephemeralSharedSecret, privateKey, ephemeralPublicKey, presharedKey);
                byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
                var keyEncryptionKey = new byte[Constants.EncryptionKeyLength];
                BLAKE2b.DeriveKey(keyEncryptionKey, ephemeralSharedSecret, Constants.Personalisation, salt);
                CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                fixed (byte* ignored = keyEncryptionKey) { EncryptInputFile(directory ? zipFilePath : inputFilePath, directory, ephemeralPublicKey, salt, keyEncryptionKey); }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
            }
        }
        CryptographicOperations.ZeroMemory(privateKey);
        CryptographicOperations.ZeroMemory(presharedKey);
        DisplayMessage.SuccessfullyEncrypted();
    }

    private static bool IsDirectory(string inputFilePath, out string zipFilePath)
    {
        bool directory = FileHandling.IsDirectory(inputFilePath);
        zipFilePath = inputFilePath + Constants.ZipFileExtension;
        if (directory) { FileHandling.CreateZipFile(inputFilePath, zipFilePath); }
        return directory;
    }
    
    private static void EncryptInputFile(string inputFilePath, bool directory, byte[] ephemeralPublicKey, byte[] salt, byte[] keyEncryptionKey)
    {
        string outputFilePath = FileHandling.GetEncryptedOutputFilePath(inputFilePath);
        DisplayMessage.EncryptingFile(inputFilePath, outputFilePath);
        EncryptFile.Encrypt(inputFilePath, outputFilePath, directory, ephemeralPublicKey, salt, keyEncryptionKey);
        CryptographicOperations.ZeroMemory(keyEncryptionKey);
    }
}