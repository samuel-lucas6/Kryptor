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
    public static void DecryptEachFileWithPassword(string[] filePaths, Span<byte> passwordBytes)
    {
        if (filePaths == null || passwordBytes == default) {
            return;
        }
        Span<byte> unencryptedHeaders = stackalloc byte[Constants.UnencryptedHeadersLength];
        Span<byte> headerKey = stackalloc byte[Constants.HeaderKeySize];
        foreach (string inputFilePath in filePaths)
        {
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
                inputFile.Read(unencryptedHeaders);
                Span<byte> salt = unencryptedHeaders[^Argon2id.SaltSize..];
                DisplayMessage.DerivingKeyFromPassword();
                Argon2id.DeriveKey(headerKey, passwordBytes, salt, Constants.Iterations, Constants.MemorySize);
                DecryptInputFile(inputFile, unencryptedHeaders, headerKey);
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
        CryptographicOperations.ZeroMemory(passwordBytes);
        DisplayMessage.SuccessfullyDecrypted(space: false);
    }
    
    public static void DecryptEachFileWithSymmetricKey(string[] filePaths, Span<byte> symmetricKey)
    {
        if (filePaths == null || symmetricKey == default) {
            return;
        }
        Span<byte> unencryptedHeaders = stackalloc byte[Constants.UnencryptedHeadersLength];
        Span<byte> headerKey = stackalloc byte[Constants.HeaderKeySize];
        foreach (string inputFilePath in filePaths)
        {
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
                inputFile.Read(unencryptedHeaders);
                Span<byte> salt = unencryptedHeaders[^BLAKE2b.SaltSize..];
                BLAKE2b.DeriveKey(headerKey, symmetricKey, Constants.Personalisation, salt);
                DecryptInputFile(inputFile, unencryptedHeaders, headerKey);
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
        DisplayMessage.SuccessfullyDecrypted(space: false);
    }

    public static void DecryptEachFileWithPublicKey(Span<byte> recipientPrivateKey, Span<byte> senderPublicKey, Span<byte> preSharedKey, string[] filePaths)
    {
        if (filePaths == null || recipientPrivateKey == default || senderPublicKey == default) {
            return;
        }
        Span<byte> sharedSecret = stackalloc byte[X25519.SharedSecretSize], ephemeralSharedSecret = stackalloc byte[X25519.SharedSecretSize];
        X25519.DeriveRecipientSharedSecret(sharedSecret, recipientPrivateKey, senderPublicKey, preSharedKey);
        Span<byte> unencryptedHeaders = stackalloc byte[Constants.UnencryptedHeadersLength];
        Span<byte> inputKeyingMaterial = stackalloc byte[ephemeralSharedSecret.Length + sharedSecret.Length];
        Span<byte> headerKey = stackalloc byte[Constants.HeaderKeySize];
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
                inputFile.Read(unencryptedHeaders);
                Span<byte> ephemeralPublicKey = unencryptedHeaders.Slice(Constants.UnencryptedHeadersLength - BLAKE2b.SaltSize - X25519.PublicKeySize, X25519.PublicKeySize);
                Span<byte> salt = unencryptedHeaders[^BLAKE2b.SaltSize..];
                X25519.DeriveRecipientSharedSecret(ephemeralSharedSecret, recipientPrivateKey, ephemeralPublicKey, preSharedKey);
                Spans.Concat(inputKeyingMaterial, ephemeralSharedSecret, sharedSecret);
                BLAKE2b.DeriveKey(headerKey, inputKeyingMaterial, Constants.Personalisation, salt);
                CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                CryptographicOperations.ZeroMemory(inputKeyingMaterial);
                DecryptInputFile(inputFile, unencryptedHeaders, headerKey);
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
        }
        CryptographicOperations.ZeroMemory(recipientPrivateKey);
        CryptographicOperations.ZeroMemory(sharedSecret);
        CryptographicOperations.ZeroMemory(preSharedKey);
        DisplayMessage.SuccessfullyDecrypted();
    }
    
    public static void DecryptEachFileWithPrivateKey(Span<byte> privateKey, Span<byte> preSharedKey, string[] filePaths)
    {
        if (filePaths == null || privateKey == default) {
            return;
        }
        Span<byte> unencryptedHeaders = stackalloc byte[Constants.UnencryptedHeadersLength];
        Span<byte> ephemeralSharedSecret = stackalloc byte[X25519.SharedSecretSize];
        Span<byte> headerKey = stackalloc byte[Constants.HeaderKeySize];
        foreach (string inputFilePath in filePaths)
        {
            Console.WriteLine();
            try
            {
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
                inputFile.Read(unencryptedHeaders);
                Span<byte> ephemeralPublicKey = unencryptedHeaders.Slice(Constants.UnencryptedHeadersLength - BLAKE2b.SaltSize - X25519.PublicKeySize, X25519.PublicKeySize);
                Span<byte> salt = unencryptedHeaders[^BLAKE2b.SaltSize..];
                X25519.DeriveSenderSharedSecret(ephemeralSharedSecret, privateKey, ephemeralPublicKey, preSharedKey);
                BLAKE2b.DeriveKey(headerKey, ephemeralSharedSecret, Constants.Personalisation, salt);
                CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
                DecryptInputFile(inputFile, unencryptedHeaders, headerKey);
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
        }
        CryptographicOperations.ZeroMemory(privateKey);
        CryptographicOperations.ZeroMemory(preSharedKey);
        DisplayMessage.SuccessfullyDecrypted();
    }
    
    private static void DecryptInputFile(FileStream inputFile, Span<byte> unencryptedHeaders, Span<byte> headerKey)
    {
        string outputFilePath = FileHandling.GetDecryptedOutputFilePath(inputFile.Name);
        DisplayMessage.DecryptingFile(inputFile.Name, outputFilePath);
        Span<byte> encryptionKey = headerKey[..ChaCha20.KeySize];
        Span<byte> nonce = headerKey[encryptionKey.Length..];
        DecryptFile.Decrypt(inputFile, outputFilePath, unencryptedHeaders, nonce, encryptionKey);
        Globals.SuccessfulCount++;
    }
}