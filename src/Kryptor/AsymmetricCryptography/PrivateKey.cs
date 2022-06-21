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
using ChaCha20BLAKE2;

namespace Kryptor;

public static class PrivateKey
{
    public static byte[] Encrypt(byte[] passwordBytes, byte[] keyAlgorithm, byte[] privateKey)
    {
        byte[] salt = SodiumCore.GetRandomBytes(Constants.SaltLength);
        DisplayMessage.DerivingKeyFromPassword();
        byte[] key = KeyDerivation.Argon2id(passwordBytes, salt);
        CryptographicOperations.ZeroMemory(passwordBytes);
        byte[] nonce = SodiumCore.GetRandomBytes(Constants.XChaChaNonceLength);
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
            return Decrypt(passwordBytes, privateKey);
        }
        catch (CryptographicException)
        {
            Console.WriteLine();
            DisplayMessage.Error("Incorrect password, or the private key has been tampered with.");
            return null;
        }
    }

    private static byte[] Decrypt(byte[] passwordBytes, byte[] privateKey)
    {
        byte[] additionalData = Arrays.Slice(privateKey, sourceIndex: 0, Constants.Curve25519KeyHeader.Length + Constants.PrivateKeyVersion.Length);
        byte[] salt = Arrays.Slice(privateKey, additionalData.Length, Constants.SaltLength);
        byte[] nonce = Arrays.Slice(privateKey, additionalData.Length + salt.Length, Constants.XChaChaNonceLength);
        byte[] encryptedPrivateKey = Arrays.SliceFromEnd(privateKey, additionalData.Length + salt.Length + nonce.Length);
        byte[] key = KeyDerivation.Argon2id(passwordBytes, salt);
        CryptographicOperations.ZeroMemory(passwordBytes);
        byte[] decryptedPrivateKey = XChaCha20BLAKE2b.Decrypt(encryptedPrivateKey, nonce, key, additionalData);
        CryptographicOperations.ZeroMemory(key);
        return decryptedPrivateKey;
    }
}