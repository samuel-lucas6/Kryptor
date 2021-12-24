/*
    Kryptor: A simple, modern, and secure encryption tool.
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
using Sodium;

namespace KryptorCLI;

public static class AsymmetricKeys
{
    public static (string publicKey, string privateKey) GenerateEncryptionKeyPair()
    {
        char[] password = PasswordPrompt.EnterNewPassword();
        byte[] passwordBytes = Password.Prehash(password);
        using var keyPair = PublicKeyBox.GenerateKeyPair();
        byte[] publicKey = Arrays.Concat(Constants.Curve25519KeyHeader, keyPair.PublicKey);
        byte[] encryptedPrivateKey = PrivateKey.Encrypt(passwordBytes, Constants.Curve25519KeyHeader, keyPair.PrivateKey);
        return (Convert.ToBase64String(publicKey), Convert.ToBase64String(encryptedPrivateKey));
    }

    public static (string publicKey, string privateKey) GenerateSigningKeyPair()
    {
        char[] password = PasswordPrompt.EnterNewPassword();
        byte[] passwordBytes = Password.Prehash(password);
        using var keyPair = PublicKeyAuth.GenerateKeyPair();
        byte[] publicKey = Arrays.Concat(Constants.Ed25519KeyHeader, keyPair.PublicKey);
        byte[] encryptedPrivateKey = PrivateKey.Encrypt(passwordBytes, Constants.Ed25519KeyHeader, keyPair.PrivateKey);
        return (Convert.ToBase64String(publicKey), Convert.ToBase64String(encryptedPrivateKey));
    }

    public static (string publicKeyPath, string privateKeyPath) ExportKeyPair(string directoryPath, string fileName, string publicKey, string privateKey)
    {
        Directory.CreateDirectory(directoryPath);
        string publicKeyPath = Path.Combine(directoryPath, fileName + Constants.PublicKeyExtension);
        CreateKeyFile(publicKeyPath, publicKey);
        string privateKeyPath = Path.Combine(directoryPath, fileName + Constants.PrivateKeyExtension);
        CreateKeyFile(privateKeyPath, privateKey);
        return (publicKeyPath, privateKeyPath);
    }

    public static string ExportPublicKey(string privateKeyFilePath, string publicKey)
    {
        string publicKeyFilePath = Path.ChangeExtension(privateKeyFilePath, extension: null) + Constants.PublicKeyExtension;
        try
        {
            if (File.Exists(publicKeyFilePath)) { return null; }
            CreateKeyFile(publicKeyFilePath, publicKey);
            return publicKeyFilePath;
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(publicKeyFilePath, ex.GetType().Name, "Unable to create a public key file.");
            return null;
        }
    }

    private static void CreateKeyFile(string filePath, string asymmetricKey)
    {
        if (File.Exists(filePath)) { File.SetAttributes(filePath, FileAttributes.Normal); }
        File.WriteAllText(filePath, asymmetricKey);
        File.SetAttributes(filePath, FileAttributes.ReadOnly);
    }

    public static byte[] GetCurve25519PublicKey(byte[] privateKey)
    {
        byte[] publicKey = ScalarMult.Base(privateKey);
        CryptographicOperations.ZeroMemory(privateKey);
        return Arrays.Concat(Constants.Curve25519KeyHeader, publicKey);
    }

    public static byte[] GetEd25519PublicKey(byte[] privateKey)
    {
        byte[] publicKey = PublicKeyAuth.ExtractEd25519PublicKeyFromEd25519SecretKey(privateKey);
        CryptographicOperations.ZeroMemory(privateKey);
        return Arrays.Concat(Constants.Ed25519KeyHeader, publicKey);
    }
}