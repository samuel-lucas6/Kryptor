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
using System.Collections.Generic;
using System.Security.Cryptography;
using Geralt;

namespace Kryptor;

public static class AsymmetricKeyValidation
{
    public static List<byte[]> EncryptionPublicKeyFile(string[] publicKeyPaths)
    {
        try
        {
            var publicKeys = new List<byte[]>();
            foreach (string publicKeyPath in publicKeyPaths)
            {
                byte[] publicKey = GetPublicKeyFromFile(publicKeyPath);
                if (publicKey == null) {
                    return null;
                }
                ValidateEncryptionKeyAlgorithm(publicKey);
                publicKeys.Add(publicKey[Constants.Curve25519KeyHeader.Length..]);
            }
            return publicKeys;
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, publicKeyPaths == null || publicKeyPaths.Length == 1 ? "Please specify a valid encryption public key." : "Please specify valid encryption public keys.");
            return null;
        }
    }

    public static Span<byte> SigningPublicKeyFile(string publicKeyPath)
    {
        try
        {
            Span<byte> publicKey = GetPublicKeyFromFile(publicKeyPath);
            if (publicKey == default) {
                return default;
            }
            ValidateSigningKeyAlgorithm(publicKey);
            return publicKey[Constants.Ed25519KeyHeader.Length..];
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            DisplayMessage.FilePathException(publicKeyPath, ex.GetType().Name, "Please specify a valid signing public key.");
            return default;
        }
    }
    
    private static byte[] GetPublicKeyFromFile(string publicKeyPath)
    {
        try
        {
            string encodedPublicKey = File.ReadAllText(publicKeyPath);
            if (encodedPublicKey.Length != Constants.PublicKeyLength) {
                throw new ArgumentException("Please specify a valid public key file.");
            }
            return Encodings.FromBase64(encodedPublicKey);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            DisplayMessage.FilePathException(publicKeyPath, ex.GetType().Name, ex is ArgumentException or FormatException ? ex.Message : "Unable to read the public key file.");
            return null;
        }
    }

    public static List<byte[]> EncryptionPublicKeyString(string[] encodedPublicKeys)
    {
        try
        {
            var publicKeys = new List<byte[]>();
            foreach (string encodedPublicKey in encodedPublicKeys)
            {
                byte[] publicKey = Encodings.FromBase64(encodedPublicKey);
                ValidateEncryptionKeyAlgorithm(publicKey);
                publicKeys.Add(publicKey[Constants.Curve25519KeyHeader.Length..]);
            }
            return publicKeys;
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, encodedPublicKeys == null || encodedPublicKeys.Length == 1 ? "Please enter a valid encryption public key." : "Please enter valid encryption public keys.");
            return null;
        }
    }

    public static Span<byte> SigningPublicKeyString(string encodedPublicKey)
    {
        try
        {
            Span<byte> publicKey = Encodings.FromBase64(encodedPublicKey);
            ValidateSigningKeyAlgorithm(publicKey);
            return publicKey[Constants.Ed25519KeyHeader.Length..];
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            DisplayMessage.KeyStringException(encodedPublicKey, ex.GetType().Name, "Please enter a valid signing public key.");
            return default;
        }
    }

    private static void ValidateEncryptionKeyAlgorithm(Span<byte> asymmetricKey)
    {
        Span<byte> keyAlgorithm = asymmetricKey[..Constants.Curve25519KeyHeader.Length];
        if (!ConstantTime.Equals(keyAlgorithm, Constants.Curve25519KeyHeader)) {
            throw new NotSupportedException("This key algorithm isn't supported for encryption.");
        }
    }

    private static void ValidateSigningKeyAlgorithm(Span<byte> asymmetricKey)
    {
        Span<byte> keyAlgorithm = asymmetricKey[..Constants.Ed25519KeyHeader.Length];
        if (!ConstantTime.Equals(keyAlgorithm, Constants.Ed25519KeyHeader)) {
            throw new NotSupportedException("This key algorithm isn't supported for signing.");
        }
    }

    public static Span<byte> EncryptionPrivateKeyFile(string privateKeyPath, Span<byte> password)
    {
        try
        {
            Span<byte> privateKey = GetPrivateKeyFromFile(privateKeyPath);
            ValidateEncryptionKeyAlgorithm(privateKey);
            return DecryptPrivateKey(privateKeyPath, password, privateKey);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            if (ex is CryptographicException) {
                DisplayMessage.Error(ex.Message);
                return default;
            }
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, ex.Message);
            return default;
        }
    }

    public static Span<byte> SigningPrivateKeyFile(string privateKeyPath, Span<byte> password)
    {
        try
        {
            Span<byte> privateKey = GetPrivateKeyFromFile(privateKeyPath);
            ValidateSigningKeyAlgorithm(privateKey);
            return DecryptPrivateKey(privateKeyPath, password, privateKey);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            if (ex is CryptographicException) {
                DisplayMessage.Error(ex.Message);
                return default;
            }
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, ex.Message);
            return default;
        }
    }

    public static Span<byte> GetPrivateKeyFromFile(string privateKeyPath)
    {
        try
        {
            string encodedPrivateKey = File.ReadAllText(privateKeyPath);
            if (encodedPrivateKey.Length != Constants.V2EncryptionPrivateKeyLength && encodedPrivateKey.Length != Constants.V2SigningPrivateKeyLength && encodedPrivateKey.Length != Constants.V1EncryptionPrivateKeyLength && encodedPrivateKey.Length != Constants.V1SigningPrivateKeyLength) {
                throw new ArgumentException("Please specify a valid private key file.");
            }
            return Encodings.FromBase64(encodedPrivateKey);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex) || ex is FormatException)
        {
            throw new ArgumentException(ex is ArgumentException or FormatException ? ex.Message : "Unable to read the private key file.", ex);
        }
    }

    public static Span<byte> DecryptPrivateKey(string privateKeyPath, Span<byte> password, Span<byte> privateKey)
    {
        Span<byte> keyVersion = privateKey.Slice(Constants.KeyAlgorithmLength, Constants.PrivateKeyVersion2.Length);
        bool version2 = ConstantTime.Equals(keyVersion, Constants.PrivateKeyVersion2);
        bool version1 = ConstantTime.Equals(keyVersion, Constants.PrivateKeyVersion1);
        if (!version2 && !version1) {
            throw new NotSupportedException("This private key version isn't supported.");
        }
        
        if (password.Length == 0) {
            password = PasswordPrompt.EnterYourPassword(isPrivateKey: true);
        }
        Console.WriteLine("Decrypting private key...");
        
        if (version2) {
            return PrivateKey.DecryptV2(password, privateKey);
        }

        Span<byte> prehashedPassword = stackalloc byte[BLAKE2b.MaxHashSize];
        BLAKE2b.ComputeHash(prehashedPassword, password);
        Span<byte> decryptedPrivateKey = PrivateKey.DecryptV1(prehashedPassword, privateKey);
        CryptographicOperations.ZeroMemory(prehashedPassword);
        
        Span<byte> unencryptedPrivateKey = GC.AllocateArray<byte>(decryptedPrivateKey.Length, pinned: true);
        decryptedPrivateKey.CopyTo(unencryptedPrivateKey);
        Span<byte> keyAlgorithm = privateKey[..Constants.KeyAlgorithmLength];
        try
        {
            Console.WriteLine("Updating private key format...");
            Span<byte> v2PrivateKey = PrivateKey.Encrypt(password, keyAlgorithm, decryptedPrivateKey);
            AsymmetricKeys.CreateKeyFile(privateKeyPath, Encodings.ToBase64(v2PrivateKey));
            Console.WriteLine("Private key format successfully updated.");
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, "Unable to update private key to latest format.");
        }
        return unencryptedPrivateKey;
    }
}