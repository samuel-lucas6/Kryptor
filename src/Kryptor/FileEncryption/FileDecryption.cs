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
using static Monocypher.Monocypher;
using Geralt;

namespace Kryptor;

public static class FileDecryption
{
    public static void DecryptEachFileWithPassword(string[] filePaths, Span<byte> password, Span<byte> pepper)
    {
        if (filePaths == null || password.Length == 0) {
            throw new UserInputException();
        }
        Span<byte> unencryptedHeaders = stackalloc byte[Constants.UnencryptedHeaderLength], wrappedFileKeys = new byte[Constants.KeyWrapHeaderLength];
        Span<byte> inputKeyingMaterial = stackalloc byte[BLAKE2b.MaxKeySize], hashedPassword = inputKeyingMaterial[..ChaCha20.KeySize];
        if (pepper.Length != 0) {
            pepper.CopyTo(inputKeyingMaterial[hashedPassword.Length..]);
            CryptographicOperations.ZeroMemory(pepper);
        }
        Span<byte> emptySalt = stackalloc byte[BLAKE2b.SaltSize]; emptySalt.Clear();
        Span<byte> headerKey = stackalloc byte[ChaCha20.KeySize];
        
        foreach (string inputFilePath in filePaths) {
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileHandling.GetFileStreamReadOptions(inputFilePath));
                inputFile.Read(unencryptedHeaders);
                inputFile.Read(wrappedFileKeys);
                
                Span<byte> salt = unencryptedHeaders[..Argon2id.SaltSize];
                Span<byte> ephemeralPublicKey = unencryptedHeaders[^X25519.PublicKeySize..];
                
                DisplayMessage.DerivingKeyFromPassword();
                Argon2id.DeriveKey(hashedPassword, password, salt, Constants.Iterations, Constants.MemorySize);
                BLAKE2b.DeriveKey(headerKey, pepper.Length == 0 ? hashedPassword : inputKeyingMaterial, Constants.Personalisation, emptySalt, info: ephemeralPublicKey);
                CryptographicOperations.ZeroMemory(hashedPassword);
                DecryptInputFile(inputFile, wrappedFileKeys, headerKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException) {
                    DisplayMessage.FilePathError(inputFilePath, ex.Message);
                }
                else {
                    DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile);
                }
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(password);
        CryptographicOperations.ZeroMemory(inputKeyingMaterial);
        DisplayMessage.SuccessfullyDecrypted();
    }
    
    public static void DecryptEachFileWithSymmetricKey(string[] filePaths, Span<byte> symmetricKey)
    {
        if (filePaths == null || symmetricKey.Length == 0) {
            throw new UserInputException();
        }
        Span<byte> unencryptedHeaders = stackalloc byte[Constants.UnencryptedHeaderLength], wrappedFileKeys = new byte[Constants.KeyWrapHeaderLength];
        Span<byte> headerKey = stackalloc byte[ChaCha20.KeySize];

        foreach (string inputFilePath in filePaths) {
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileHandling.GetFileStreamReadOptions(inputFilePath));
                inputFile.Read(unencryptedHeaders);
                inputFile.Read(wrappedFileKeys);

                Span<byte> salt = unencryptedHeaders[..BLAKE2b.SaltSize];
                Span<byte> ephemeralPublicKey = unencryptedHeaders[^X25519.PublicKeySize..];

                BLAKE2b.DeriveKey(headerKey, symmetricKey, Constants.Personalisation, salt, info: ephemeralPublicKey);
                DecryptInputFile(inputFile, wrappedFileKeys, headerKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException) {
                    DisplayMessage.FilePathError(inputFilePath, ex.Message);
                }
                else {
                    DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile);
                }
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(symmetricKey);
        DisplayMessage.SuccessfullyDecrypted();
    }

    public static void DecryptEachFileWithPublicKey(string[] filePaths, Span<byte> recipientPrivateKey, Span<byte> senderPublicKey, Span<byte> preSharedKey)
    {
        if (filePaths == null || recipientPrivateKey.Length == 0 || senderPublicKey.Length == 0) {
            throw new UserInputException();
        }
        Span<byte> sharedSecret = stackalloc byte[X25519.SharedSecretSize], ephemeralSharedSecret = stackalloc byte[X25519.SharedSecretSize];
        X25519.DeriveRecipientSharedSecret(sharedSecret, recipientPrivateKey, senderPublicKey, preSharedKey);
        
        Span<byte> unencryptedHeaders = stackalloc byte[Constants.UnencryptedHeaderLength], wrappedFileKeys = new byte[Constants.KeyWrapHeaderLength];
        Span<byte> unhiddenEphemeralPublicKey = stackalloc byte[X25519.PublicKeySize];
        Span<byte> inputKeyingMaterial = stackalloc byte[ephemeralSharedSecret.Length + sharedSecret.Length];
        Span<byte> headerKey = stackalloc byte[ChaCha20.KeySize];

        foreach (string inputFilePath in filePaths) {
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileHandling.GetFileStreamReadOptions(inputFilePath));
                inputFile.Read(unencryptedHeaders);
                inputFile.Read(wrappedFileKeys);

                Span<byte> salt = unencryptedHeaders[..BLAKE2b.SaltSize];
                Span<byte> ephemeralPublicKey = unencryptedHeaders[^X25519.PublicKeySize..];
                
                crypto_hidden_to_curve(unhiddenEphemeralPublicKey, ephemeralPublicKey);
                X25519.DeriveRecipientSharedSecret(ephemeralSharedSecret, recipientPrivateKey, unhiddenEphemeralPublicKey, preSharedKey);
                Spans.Concat(inputKeyingMaterial, ephemeralSharedSecret, sharedSecret);
                BLAKE2b.DeriveKey(headerKey, inputKeyingMaterial, Constants.Personalisation, salt, info: ephemeralPublicKey);
                CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                CryptographicOperations.ZeroMemory(inputKeyingMaterial);
                
                DecryptInputFile(inputFile, wrappedFileKeys, headerKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException) {
                    DisplayMessage.FilePathError(inputFilePath, ex.Message);
                }
                else {
                    DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile);
                }
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(recipientPrivateKey);
        CryptographicOperations.ZeroMemory(sharedSecret);
        CryptographicOperations.ZeroMemory(preSharedKey);
        DisplayMessage.SuccessfullyDecrypted();
    }
    
    public static void DecryptEachFileWithPrivateKey(string[] filePaths, Span<byte> privateKey, Span<byte> preSharedKey)
    {
        if (filePaths == null || privateKey.Length == 0) {
            throw new UserInputException();
        }
        Span<byte> unencryptedHeaders = stackalloc byte[Constants.UnencryptedHeaderLength], wrappedFileKeys = new byte[Constants.KeyWrapHeaderLength];
        Span<byte> unhiddenEphemeralPublicKey = stackalloc byte[X25519.PublicKeySize];
        Span<byte> ephemeralSharedSecret = stackalloc byte[X25519.SharedSecretSize];
        Span<byte> headerKey = stackalloc byte[ChaCha20.KeySize];

        foreach (string inputFilePath in filePaths) {
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileHandling.GetFileStreamReadOptions(inputFilePath));
                inputFile.Read(unencryptedHeaders);
                inputFile.Read(wrappedFileKeys);
                
                Span<byte> salt = unencryptedHeaders[..BLAKE2b.SaltSize];
                Span<byte> ephemeralPublicKey = unencryptedHeaders[^X25519.PublicKeySize..];
                
                crypto_hidden_to_curve(unhiddenEphemeralPublicKey, ephemeralPublicKey);
                X25519.DeriveSenderSharedSecret(ephemeralSharedSecret, privateKey, unhiddenEphemeralPublicKey, preSharedKey);
                BLAKE2b.DeriveKey(headerKey, ephemeralSharedSecret, Constants.Personalisation, salt, info: ephemeralPublicKey);
                CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                
                DecryptInputFile(inputFile, wrappedFileKeys, headerKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                if (ex is ArgumentException) {
                    DisplayMessage.FilePathError(inputFilePath, ex.Message);
                }
                else {
                    DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, ErrorMessages.UnableToDecryptFile);
                }
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(privateKey);
        CryptographicOperations.ZeroMemory(preSharedKey);
        DisplayMessage.SuccessfullyDecrypted();
    }
    
    private static void DecryptInputFile(FileStream inputFile, Span<byte> wrappedFileKeys, Span<byte> headerKey)
    {
        string outputFilePath = FileHandling.GetUniqueFilePath(FileHandling.RemoveFileNameNumber(Path.ChangeExtension(inputFile.Name, extension: null)));
        DisplayMessage.InputToOutput("Decrypting", inputFile.Name, outputFilePath);
        
        Span<byte> fileKeys = GC.AllocateArray<byte>(Constants.KeyWrapHeaderLength, pinned: true);
        Span<byte> nonce = stackalloc byte[ChaCha20.NonceSize]; nonce.Clear();

        for (int i = 0; i < wrappedFileKeys.Length; i += ChaCha20.KeySize) {
            ChaCha20.Decrypt(fileKeys.Slice(i, ChaCha20.KeySize), wrappedFileKeys.Slice(i, ChaCha20.KeySize), nonce, headerKey, Constants.ChaCha20Counter);
        }
        CryptographicOperations.ZeroMemory(headerKey);
        
        for (int i = 0; i < fileKeys.Length; i += ChaCha20.KeySize) {
            try
            {
                DecryptFile.Decrypt(inputFile, outputFilePath, wrappedFileKeys, fileKeys.Slice(i, ChaCha20.KeySize));
                CryptographicOperations.ZeroMemory(fileKeys);
                Globals.SuccessfulCount++;
                break;
            }
            catch (ArgumentException)
            {
                inputFile.Position -= Constants.EncryptedHeaderLength;
                if (i == fileKeys.Length - ChaCha20.KeySize) {
                    throw;
                }
            }
        }
    }
}