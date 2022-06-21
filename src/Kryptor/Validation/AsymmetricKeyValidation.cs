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
using Sodium;

namespace Kryptor;

public static class AsymmetricKeyValidation
{
    public static byte[] EncryptionPublicKeyFile(string publicKeyPath)
    {
        try
        {
            byte[] publicKey = GetPublicKeyFromFile(publicKeyPath);
            if (publicKey == null) { return null; }
            ValidateEncryptionKeyAlgorithm(publicKey);
            return Arrays.SliceFromEnd(publicKey, Constants.Curve25519KeyHeader.Length);
        }
        catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, "Please specify a valid encryption public key.");
            return null;
        }
    }

    public static byte[] SigningPublicKeyFile(string publicKeyPath)
    {
        try
        {
            byte[] publicKey = GetPublicKeyFromFile(publicKeyPath);
            if (publicKey == null) { return null; }
            ValidateSigningKeyAlgorithm(publicKey);
            return Arrays.SliceFromEnd(publicKey, Constants.Ed25519KeyHeader.Length);
        }
        catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, "Please specify a valid signing public key.");
            return null;
        }
    }

    private static byte[] GetPublicKeyFromFile(string publicKeyPath)
    {
        try
        {
            string encodedPublicKey = File.ReadAllText(publicKeyPath);
            if (encodedPublicKey.Length == Constants.PublicKeyLength) { return Convert.FromBase64String(encodedPublicKey); }
            DisplayMessage.Error(ErrorMessages.InvalidPublicKey);
            return null;
        }
        catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, "Unable to retrieve the public key.");
            return null;
        }
    }

    public static byte[] EncryptionPublicKeyString(char[] encodedPublicKey)
    {
        try
        {
            byte[] publicKey = Convert.FromBase64CharArray(encodedPublicKey, offset: 0, encodedPublicKey.Length);
            ValidateEncryptionKeyAlgorithm(publicKey);
            return Arrays.SliceFromEnd(publicKey, Constants.Curve25519KeyHeader.Length);
        }
        catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, "Please enter a valid encryption public key.");
            return null;
        }
    }

    public static byte[] SigningPublicKeyString(char[] encodedPublicKey)
    {
        try
        {
            byte[] publicKey = Convert.FromBase64CharArray(encodedPublicKey, offset: 0, encodedPublicKey.Length);
            ValidateSigningKeyAlgorithm(publicKey);
            return Arrays.SliceFromEnd(publicKey, Constants.Ed25519KeyHeader.Length);
        }
        catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, "Please enter a valid signing public key.");
            return null;
        }
    }

    private static void ValidateEncryptionKeyAlgorithm(byte[] asymmetricKey)
    {
        byte[] keyAlgorithm = Arrays.Slice(asymmetricKey, sourceIndex: 0, Constants.Curve25519KeyHeader.Length);
        bool validKey = Utilities.Compare(keyAlgorithm, Constants.Curve25519KeyHeader);
        if (!validKey) { throw new ArgumentException("Please specify an asymmetric encryption key."); }
    }

    private static void ValidateSigningKeyAlgorithm(byte[] asymmetricKey)
    {
        byte[] keyAlgorithm = Arrays.Slice(asymmetricKey, sourceIndex: 0, Constants.Ed25519KeyHeader.Length);
        bool validKey = Utilities.Compare(keyAlgorithm, Constants.Ed25519KeyHeader);
        if (!validKey) { throw new ArgumentException("Please specify an asymmetric signing key."); }
    }

    public static byte[] EncryptionPrivateKeyFile(string privateKeyPath)
    {
        try
        {
            byte[] privateKey = GetPrivateKeyFromFile(privateKeyPath);
            if (privateKey == null) { return null; }
            ValidateEncryptionKeyAlgorithm(privateKey);
            return privateKey;
        }
        catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, "Please specify a valid encryption private key.");
            return null;
        }
    }

    public static byte[] SigningPrivateKeyFile(string privateKeyPath)
    {
        try
        {
            byte[] privateKey = GetPrivateKeyFromFile(privateKeyPath);
            if (privateKey == null) { return null; }
            ValidateSigningKeyAlgorithm(privateKey);
            return privateKey;
        }
        catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, "Please specify a valid signing private key.");
            return null;
        }
    }

    public static byte[] GetPrivateKeyFromFile(string privateKeyPath)
    {
        try
        {
            string encodedPrivateKey = File.ReadAllText(privateKeyPath);
            if (encodedPrivateKey.Length != Constants.EncryptionPrivateKeyLength && encodedPrivateKey.Length != Constants.SigningPrivateKeyLength)
            {
                DisplayMessage.Error(ErrorMessages.InvalidPrivateKeyFile);
                return null;
            }
            byte[] privateKey = Convert.FromBase64String(encodedPrivateKey);
            ValidateKeyVersion(privateKey);
            return privateKey;
        }
        catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
        {
            if (ex is ArgumentException)
            {
                DisplayMessage.Exception(ex.GetType().Name, ex.Message);
                return null;
            }
            DisplayMessage.Exception(ex.GetType().Name, "Unable to retrieve the private key.");
            return null;
        }
    }

    private static void ValidateKeyVersion(byte[] privateKey)
    {
        byte[] keyVersion = Arrays.Slice(privateKey, Constants.Curve25519KeyHeader.Length, Constants.PrivateKeyVersion.Length);
        bool validKeyVersion = Utilities.Compare(keyVersion, Constants.PrivateKeyVersion);
        if (!validKeyVersion) { throw new ArgumentException("This private key version is not supported in this version of Kryptor."); }
    }
}