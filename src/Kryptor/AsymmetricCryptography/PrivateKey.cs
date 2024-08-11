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
using System.Security.Cryptography;
using Geralt;
using kcAEAD;

namespace Kryptor;

public static class PrivateKey
{
    public static Span<byte> Encrypt(Span<byte> privateKey, Span<byte> passphrase, Span<byte> keyAlgorithm)
    {
        DisplayMessage.DerivingKeyFromPassphrase();
        Span<byte> salt = stackalloc byte[Argon2id.SaltSize];
        SecureRandom.Fill(salt);

        Span<byte> nonce = stackalloc byte[ChaCha20.NonceSize]; nonce.Clear();
        Span<byte> key = stackalloc byte[ChaCha20.KeySize];
        Argon2id.DeriveKey(key, passphrase, salt, Constants.Iterations, Constants.MemorySize);
        CryptographicOperations.ZeroMemory(passphrase);

        Span<byte> associatedData = stackalloc byte[keyAlgorithm.Length + Constants.PrivateKeyVersion.Length];
        Spans.Concat(associatedData, keyAlgorithm, Constants.PrivateKeyVersion);

        Span<byte> encryptedPrivateKey = stackalloc byte[kcChaCha20Poly1305.CommitmentSize + privateKey.Length + Poly1305.TagSize];
        kcChaCha20Poly1305.Encrypt(encryptedPrivateKey, privateKey, nonce, key, associatedData);
        CryptographicOperations.ZeroMemory(privateKey);
        CryptographicOperations.ZeroMemory(key);

        Span<byte> fullPrivateKey = new byte[associatedData.Length + salt.Length + encryptedPrivateKey.Length];
        Spans.Concat(fullPrivateKey, associatedData, salt, encryptedPrivateKey);
        return fullPrivateKey;
    }

    public static Span<byte> Decrypt(Span<byte> privateKey, Span<byte> passphrase)
    {
        try
        {
            Span<byte> associatedData = privateKey[..(Constants.Curve25519KeyHeader.Length + Constants.PrivateKeyVersion.Length)];
            Span<byte> salt = privateKey.Slice(associatedData.Length, Argon2id.SaltSize);
            Span<byte> encryptedPrivateKey = privateKey[(associatedData.Length + salt.Length)..];

            Span<byte> nonce = stackalloc byte[ChaCha20.NonceSize]; nonce.Clear();
            Span<byte> key = stackalloc byte[ChaCha20.KeySize];
            Argon2id.DeriveKey(key, passphrase, salt, Constants.Iterations, Constants.MemorySize);
            CryptographicOperations.ZeroMemory(passphrase);

            Span<byte> decryptedPrivateKey = GC.AllocateArray<byte>(encryptedPrivateKey.Length - Poly1305.TagSize - kcChaCha20Poly1305.CommitmentSize, pinned: true);
            kcChaCha20Poly1305.Decrypt(decryptedPrivateKey, encryptedPrivateKey, nonce, key, associatedData);
            CryptographicOperations.ZeroMemory(key);
            return decryptedPrivateKey;
        }
        catch (CryptographicException ex)
        {
            throw new CryptographicException("Incorrect passphrase, or the private key has been tampered with.", ex);
        }
    }
}
