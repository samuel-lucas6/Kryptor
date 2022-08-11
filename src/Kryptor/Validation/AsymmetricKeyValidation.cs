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
using System.IO;
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
                publicKeys.Add(Arrays.SliceFromEnd(publicKey, Constants.Curve25519KeyHeader.Length));
            }
            return publicKeys;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            DisplayMessage.Exception(ex.GetType().Name, publicKeyPaths == null || publicKeyPaths.Length == 1 ? "Please specify a valid encryption public key." : "Please specify valid encryption public keys.");
            return null;
        }
    }

    public static byte[] SigningPublicKeyFile(string publicKeyPath)
    {
        try
        {
            byte[] publicKey = GetPublicKeyFromFile(publicKeyPath);
            if (publicKey == null) {
                return null;
            }
            ValidateSigningKeyAlgorithm(publicKey);
            return Arrays.SliceFromEnd(publicKey, Constants.Ed25519KeyHeader.Length);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            DisplayMessage.FilePathException(publicKeyPath, ex.GetType().Name, "Please specify a valid signing public key.");
            return null;
        }
    }
    
    private static byte[] GetPublicKeyFromFile(string publicKeyPath)
    {
        try
        {
            string encodedPublicKey = File.ReadAllText(publicKeyPath);
            if (encodedPublicKey.Length == Constants.PublicKeyLength) {
                return Encodings.FromBase64(encodedPublicKey);
            }
            DisplayMessage.FilePathError(publicKeyPath, ErrorMessages.InvalidPublicKey);
            return null;
        }
        catch (Exception ex)
        {
            DisplayMessage.FilePathException(publicKeyPath, ex.GetType().Name, "Unable to retrieve the public key, or the public key is invalid.");
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
                publicKeys.Add(Arrays.SliceFromEnd(publicKey, Constants.Curve25519KeyHeader.Length));
            }
            return publicKeys;
        }
        catch (Exception ex)
        {
            DisplayMessage.Exception(ex.GetType().Name, encodedPublicKeys == null || encodedPublicKeys.Length == 1 ? "Please enter a valid encryption public key." : "Please enter valid encryption public keys.");
            return null;
        }
    }

    public static byte[] SigningPublicKeyString(string encodedPublicKey)
    {
        try
        {
            byte[] publicKey = Encodings.FromBase64(encodedPublicKey);
            ValidateSigningKeyAlgorithm(publicKey);
            return Arrays.SliceFromEnd(publicKey, Constants.Ed25519KeyHeader.Length);
        }
        catch (Exception ex)
        {
            DisplayMessage.KeyStringException(encodedPublicKey, ex.GetType().Name, "Please enter a valid signing public key.");
            return null;
        }
    }

    private static void ValidateEncryptionKeyAlgorithm(byte[] asymmetricKey)
    {
        byte[] keyAlgorithm = Arrays.Slice(asymmetricKey, sourceIndex: 0, Constants.Curve25519KeyHeader.Length);
        bool validKey = ConstantTime.Equals(keyAlgorithm, Constants.Curve25519KeyHeader);
        if (!validKey) {
            throw new NotSupportedException("This key algorithm isn't supported for encryption.");
        }
    }

    private static void ValidateSigningKeyAlgorithm(byte[] asymmetricKey)
    {
        byte[] keyAlgorithm = Arrays.Slice(asymmetricKey, sourceIndex: 0, Constants.Ed25519KeyHeader.Length);
        bool validKey = ConstantTime.Equals(keyAlgorithm, Constants.Ed25519KeyHeader);
        if (!validKey) {
            throw new NotSupportedException("This key algorithm isn't supported for signing.");
        }
    }

    public static byte[] EncryptionPrivateKeyFile(string privateKeyPath)
    {
        try
        {
            byte[] privateKey = GetPrivateKeyFromFile(privateKeyPath);
            if (privateKey == null) {
                return null;
            }
            ValidateEncryptionKeyAlgorithm(privateKey);
            return privateKey;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, "Please specify a valid encryption private key.");
            return null;
        }
    }

    public static byte[] SigningPrivateKeyFile(string privateKeyPath)
    {
        try
        {
            byte[] privateKey = GetPrivateKeyFromFile(privateKeyPath);
            if (privateKey == null) {
                return null;
            }
            ValidateSigningKeyAlgorithm(privateKey);
            return privateKey;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, "Please specify a valid signing private key.");
            return null;
        }
    }

    public static byte[] GetPrivateKeyFromFile(string privateKeyPath)
    {
        try
        {
            string encodedPrivateKey = File.ReadAllText(privateKeyPath);
            if (encodedPrivateKey.Length != Constants.EncryptionPrivateKeyLength && encodedPrivateKey.Length != Constants.SigningPrivateKeyLength) {
                DisplayMessage.FilePathError(privateKeyPath, "Please specify a valid private key file.");
                return null;
            }
            byte[] privateKey = Encodings.FromBase64(encodedPrivateKey);
            ValidateKeyVersion(privateKey);
            return privateKey;
        }
        catch (Exception ex)
        {
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, ex is NotSupportedException ? ex.Message : "Unable to retrieve the private key, or the private key is invalid.");
            return null;
        }
    }

    private static void ValidateKeyVersion(byte[] privateKey)
    {
        byte[] keyVersion = Arrays.Slice(privateKey, Constants.Curve25519KeyHeader.Length, Constants.PrivateKeyVersion.Length);
        bool validKeyVersion = ConstantTime.Equals(keyVersion, Constants.PrivateKeyVersion);
        if (!validKeyVersion) {
            throw new NotSupportedException("This private key version isn't supported.");
        }
    }
}