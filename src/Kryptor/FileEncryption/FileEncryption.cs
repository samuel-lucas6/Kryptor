/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2023 Samuel Lucas

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
using System.Security.Cryptography;
using static Monocypher.Monocypher;
using Geralt;

namespace Kryptor;

public static class FileEncryption
{
    public static void EncryptEachFileWithPassphrase(string[] filePaths, Span<byte> passphrase, Span<byte> pepper)
    {
        if (filePaths == null || passphrase.Length == 0) {
            throw new UserInputException();
        }
        Span<byte> salt = stackalloc byte[Argon2id.SaltSize];
        Span<byte> ephemeralPublicKey = stackalloc byte[X25519.PublicKeySize];
        Span<byte> inputKeyingMaterial = stackalloc byte[BLAKE2b.MaxKeySize], hashedPassphrase = inputKeyingMaterial[..ChaCha20.KeySize];
        if (pepper.Length != 0) {
            pepper.CopyTo(inputKeyingMaterial[hashedPassphrase.Length..]);
            CryptographicOperations.ZeroMemory(pepper);
        }
        Span<byte> emptySalt = stackalloc byte[BLAKE2b.SaltSize]; emptySalt.Clear();
        Span<byte> headerKey = stackalloc byte[ChaCha20.KeySize];
        Span<byte> fileKey = stackalloc byte[ChaCha20.KeySize], wrappedFileKeys = new byte[Constants.KeyWrapHeaderLength];
        Span<byte> nonce = stackalloc byte[ChaCha20.NonceSize]; nonce.Clear();
        
        foreach (string inputFilePath in filePaths) {
            try
            {
                bool isDirectory = IsDirectory(inputFilePath, out string zipFilePath);
                SecureRandom.Fill(salt);
                SecureRandom.Fill(ephemeralPublicKey);
                
                DisplayMessage.DerivingKeyFromPassphrase();
                Argon2id.DeriveKey(hashedPassphrase, passphrase, salt, Constants.Iterations, Constants.MemorySize);
                BLAKE2b.DeriveKey(headerKey, pepper.Length == 0 ? hashedPassphrase : inputKeyingMaterial, Constants.Personalisation, emptySalt, info: ephemeralPublicKey);
                CryptographicOperations.ZeroMemory(hashedPassphrase);
                
                SecureRandom.Fill(fileKey);
                ChaCha20.Encrypt(wrappedFileKeys[..fileKey.Length], fileKey, nonce, headerKey);
                CryptographicOperations.ZeroMemory(headerKey);
                SecureRandom.Fill(wrappedFileKeys[fileKey.Length..]);
                
                EncryptInputFile(isDirectory ? zipFilePath : inputFilePath, isDirectory, salt, ephemeralPublicKey, wrappedFileKeys, fileKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(passphrase);
        CryptographicOperations.ZeroMemory(inputKeyingMaterial);
        DisplayMessage.SuccessfullyEncrypted();
    }
    
    public static void EncryptEachFileWithSymmetricKey(string[] filePaths, Span<byte> symmetricKey)
    {
        if (filePaths == null || symmetricKey.Length == 0) {
            throw new UserInputException();
        }
        Span<byte> salt = stackalloc byte[BLAKE2b.SaltSize];
        Span<byte> ephemeralPublicKey = stackalloc byte[X25519.PublicKeySize];
        Span<byte> headerKey = stackalloc byte[ChaCha20.KeySize];
        Span<byte> fileKey = stackalloc byte[ChaCha20.KeySize], wrappedFileKeys = new byte[Constants.KeyWrapHeaderLength];
        Span<byte> nonce = stackalloc byte[ChaCha20.NonceSize]; nonce.Clear();
        
        foreach (string inputFilePath in filePaths) {
            try
            {
                bool isDirectory = IsDirectory(inputFilePath, out string zipFilePath);
                SecureRandom.Fill(salt);
                SecureRandom.Fill(ephemeralPublicKey);
                BLAKE2b.DeriveKey(headerKey, symmetricKey, Constants.Personalisation, salt, info: ephemeralPublicKey);
                
                SecureRandom.Fill(fileKey);
                ChaCha20.Encrypt(wrappedFileKeys[..fileKey.Length], fileKey, nonce, headerKey);
                CryptographicOperations.ZeroMemory(headerKey);
                SecureRandom.Fill(wrappedFileKeys[fileKey.Length..]);
                
                EncryptInputFile(isDirectory ? zipFilePath : inputFilePath, isDirectory, salt, ephemeralPublicKey, wrappedFileKeys, fileKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(symmetricKey);
        DisplayMessage.SuccessfullyEncrypted();
    }

    public static void EncryptEachFileWithPublicKey(string[] filePaths, Span<byte> senderPrivateKey, List<byte[]> recipientPublicKeys, Span<byte> preSharedKey)
    {
        if (filePaths == null || senderPrivateKey.Length == 0 || recipientPublicKeys == null) {
            throw new UserInputException();
        }
        Span<byte> ephemeralSharedSecret = stackalloc byte[X25519.SharedSecretSize], sharedSecret = stackalloc byte[X25519.SharedSecretSize];
        Span<byte> seed = stackalloc byte[X25519.SeedSize], xCoordinate = stackalloc byte[X25519.SharedSecretSize];
        Span<byte> ephemeralPublicKey = stackalloc byte[X25519.PublicKeySize], ephemeralPrivateKey = stackalloc byte[X25519.PrivateKeySize];
        Span<byte> unhiddenEphemeralPublicKey = stackalloc byte[X25519.PublicKeySize];
        Span<byte> salt = stackalloc byte[BLAKE2b.SaltSize];
        Span<byte> inputKeyingMaterial = stackalloc byte[ephemeralSharedSecret.Length + sharedSecret.Length];
        Span<byte> headerKey = stackalloc byte[ChaCha20.KeySize];
        Span<byte> fileKey = stackalloc byte[ChaCha20.KeySize], wrappedFileKeys = new byte[Constants.KeyWrapHeaderLength];
        Span<byte> nonce = stackalloc byte[ChaCha20.NonceSize]; nonce.Clear();

        foreach (string inputFilePath in filePaths) {
            try
            {
                bool isDirectory = IsDirectory(inputFilePath, out string zipFilePath);

                SecureRandom.Fill(salt);
                SecureRandom.Fill(seed);
                crypto_hidden_key_pair(ephemeralPublicKey, ephemeralPrivateKey, seed);
                crypto_hidden_to_curve(unhiddenEphemeralPublicKey, ephemeralPublicKey);

                int wrappedKeyIndex = 0;
                SecureRandom.Fill(fileKey);
                foreach (Span<byte> recipientPublicKey in recipientPublicKeys) {
                    X25519.ComputeSharedSecret(xCoordinate, ephemeralPrivateKey, recipientPublicKey);
                    using var blake2b = new IncrementalBLAKE2b(ephemeralSharedSecret.Length, preSharedKey);
                    blake2b.Update(xCoordinate);
                    blake2b.Update(unhiddenEphemeralPublicKey);
                    blake2b.Update(recipientPublicKey);
                    blake2b.Finalize(ephemeralSharedSecret);
                    CryptographicOperations.ZeroMemory(xCoordinate);

                    X25519.DeriveSenderSharedKey(sharedSecret, senderPrivateKey, recipientPublicKey, preSharedKey);
                    Spans.Concat(inputKeyingMaterial, ephemeralSharedSecret, sharedSecret);
                    BLAKE2b.DeriveKey(headerKey, inputKeyingMaterial, Constants.Personalisation, salt, info: ephemeralPublicKey);
                    CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                    CryptographicOperations.ZeroMemory(sharedSecret);
                    CryptographicOperations.ZeroMemory(inputKeyingMaterial);
                    
                    ChaCha20.Encrypt(wrappedFileKeys.Slice(wrappedKeyIndex, fileKey.Length), fileKey, nonce, headerKey);
                    CryptographicOperations.ZeroMemory(headerKey);
                    wrappedKeyIndex += fileKey.Length;
                }
                CryptographicOperations.ZeroMemory(ephemeralPrivateKey);

                if (wrappedKeyIndex != wrappedFileKeys.Length) {
                    SecureRandom.Fill(wrappedFileKeys[wrappedKeyIndex..]);
                }
                EncryptInputFile(isDirectory ? zipFilePath : inputFilePath, isDirectory, salt, ephemeralPublicKey, wrappedFileKeys, fileKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(senderPrivateKey);
        CryptographicOperations.ZeroMemory(preSharedKey);
        DisplayMessage.SuccessfullyEncrypted();
    }

    public static void EncryptEachFileWithPrivateKey(string[] filePaths, Span<byte> privateKey, Span<byte> preSharedKey)
    {
        if (filePaths == null || privateKey.Length == 0) {
            throw new UserInputException();
        }
        Span<byte> seed = stackalloc byte[X25519.SeedSize];
        Span<byte> ephemeralPublicKey = stackalloc byte[X25519.PublicKeySize], ephemeralPrivateKey = stackalloc byte[X25519.PrivateKeySize];
        Span<byte> unhiddenEphemeralPublicKey = stackalloc byte[X25519.PublicKeySize];
        Span<byte> ephemeralSharedSecret = stackalloc byte[X25519.SharedSecretSize];
        Span<byte> salt = stackalloc byte[BLAKE2b.SaltSize];
        Span<byte> headerKey = stackalloc byte[ChaCha20.KeySize];
        Span<byte> fileKey = stackalloc byte[ChaCha20.KeySize], wrappedFileKeys = new byte[Constants.KeyWrapHeaderLength];
        Span<byte> nonce = stackalloc byte[ChaCha20.NonceSize]; nonce.Clear();
        
        foreach (string inputFilePath in filePaths) {
            try
            {
                bool isDirectory = IsDirectory(inputFilePath, out string zipFilePath);
                
                SecureRandom.Fill(seed);
                crypto_hidden_key_pair(ephemeralPublicKey, ephemeralPrivateKey, seed);
                CryptographicOperations.ZeroMemory(ephemeralPrivateKey);
                crypto_hidden_to_curve(unhiddenEphemeralPublicKey, ephemeralPublicKey);
                
                SecureRandom.Fill(salt);
                X25519.DeriveSenderSharedKey(ephemeralSharedSecret, privateKey, unhiddenEphemeralPublicKey, preSharedKey);
                BLAKE2b.DeriveKey(headerKey, ephemeralSharedSecret, Constants.Personalisation, salt, info: ephemeralPublicKey);
                CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                
                SecureRandom.Fill(fileKey);
                ChaCha20.Encrypt(wrappedFileKeys[..fileKey.Length], fileKey, nonce, headerKey);
                CryptographicOperations.ZeroMemory(headerKey);
                SecureRandom.Fill(wrappedFileKeys[fileKey.Length..]);
                
                EncryptInputFile(isDirectory ? zipFilePath : inputFilePath, isDirectory, salt, ephemeralPublicKey, wrappedFileKeys, fileKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToEncryptFile);
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(privateKey);
        CryptographicOperations.ZeroMemory(preSharedKey);
        DisplayMessage.SuccessfullyEncrypted();
    }

    private static bool IsDirectory(string inputFilePath, out string zipFilePath)
    {
        bool isDirectory = File.GetAttributes(inputFilePath).HasFlag(FileAttributes.Directory);
        zipFilePath = FileHandling.TrimTrailingSeparatorChars(inputFilePath) + Constants.ZipFileExtension;
        if (isDirectory) {
            FileHandling.CreateZipFile(inputFilePath, zipFilePath);
        }
        return isDirectory;
    }
    
    private static void EncryptInputFile(string inputFilePath, bool isDirectory, Span<byte> salt, Span<byte> ephemeralPublicKey, Span<byte> wrappedFileKeys, Span<byte> fileKey)
    {
        string outputFilePath = !Globals.EncryptFileNames ? inputFilePath + Constants.EncryptedExtension : FileHandling.ReplaceFileName(inputFilePath, SecureRandom.GetString(Constants.RandomFileNameLength));
        outputFilePath = FileHandling.GetUniqueFilePath(outputFilePath);
        DisplayMessage.InputToOutput("Encrypting", inputFilePath, outputFilePath);
        
        Span<byte> unencryptedHeader = stackalloc byte[Constants.UnencryptedHeaderLength];
        Spans.Concat(unencryptedHeader, salt, ephemeralPublicKey);

        EncryptFile.Encrypt(inputFilePath, outputFilePath, isDirectory, unencryptedHeader, wrappedFileKeys, fileKey);
        Globals.SuccessfulCount++;
    }
}