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
using Geralt;
using ChaCha20BLAKE2;

namespace Kryptor;

public static class PrivateKey
{
    public static byte[] Encrypt(byte[] passwordBytes, byte[] keyAlgorithm, byte[] privateKey)
    {
        var salt = new byte[Constants.SaltLength];
        SecureRandom.Fill(salt);
        DisplayMessage.DerivingKeyFromPassword();
        var key = GC.AllocateArray<byte>(Constants.EncryptionKeyLength, pinned: true);
        Argon2id.DeriveKey(key, passwordBytes, salt, Constants.Iterations, Constants.MemorySize);
        CryptographicOperations.ZeroMemory(passwordBytes);
        var nonce = new byte[Constants.XChaChaNonceLength];
        SecureRandom.Fill(nonce);
        byte[] additionalData = Arrays.Concat(keyAlgorithm, Constants.PrivateKeyVersion);
        byte[] encryptedPrivateKey = XChaCha20BLAKE2b.Encrypt(privateKey, nonce, key, additionalData);
        CryptographicOperations.ZeroMemory(privateKey);
        CryptographicOperations.ZeroMemory(key);
        return Arrays.Concat(additionalData, salt, nonce, encryptedPrivateKey);
    }

    public static byte[] Decrypt(byte[] privateKey, char[] password)
    {
        if (privateKey == null) { return null; }
        try
        {
            if (password.Length == 0) { password = PasswordPrompt.EnterYourPassword(isPrivateKey: true); }
            Console.WriteLine("Decrypting private key...");
            var passwordBytes = Password.Prehash(password);
            byte[] additionalData = Arrays.Slice(privateKey, sourceIndex: 0, Constants.Curve25519KeyHeader.Length + Constants.PrivateKeyVersion.Length);
            byte[] salt = Arrays.Slice(privateKey, additionalData.Length, Constants.SaltLength);
            byte[] nonce = Arrays.Slice(privateKey, additionalData.Length + salt.Length, Constants.XChaChaNonceLength);
            byte[] encryptedPrivateKey = Arrays.SliceFromEnd(privateKey, additionalData.Length + salt.Length + nonce.Length);
            var key = GC.AllocateArray<byte>(Constants.EncryptionKeyLength, pinned: true);
            Argon2id.DeriveKey(key, passwordBytes, salt, Constants.Iterations, Constants.MemorySize);
            CryptographicOperations.ZeroMemory(passwordBytes);
            byte[] decryptedPrivateKey = XChaCha20BLAKE2b.Decrypt(encryptedPrivateKey, nonce, key, additionalData);
            CryptographicOperations.ZeroMemory(key);
            return decryptedPrivateKey;
        }
        catch (CryptographicException)
        {
            Console.WriteLine();
            DisplayMessage.Error("Incorrect password, or the private key has been tampered with.");
            return null;
        }
    }
}