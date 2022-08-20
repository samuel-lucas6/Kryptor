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

public static class AsymmetricKeys
{
    public static (string publicKey, string privateKey) GenerateEncryptionKeyPair(char[] password)
    {
        password = Password.GetNewPassword(password);
        var passwordBytes = Password.Prehash(password);
        var publicKey = new byte[X25519.PublicKeySize];
        var privateKey = GC.AllocateArray<byte>(X25519.PrivateKeySize, pinned: true);
        X25519.GenerateKeyPair(publicKey, privateKey);
        publicKey = Arrays.Concat(Constants.Curve25519KeyHeader, publicKey);
        Span<byte> encryptedPrivateKey = PrivateKey.Encrypt(passwordBytes, Constants.Curve25519KeyHeader, privateKey);
        return (Encodings.ToBase64(publicKey), Encodings.ToBase64(encryptedPrivateKey));
    }

    public static (string publicKey, string privateKey) GenerateSigningKeyPair(char[] password)
    {
        password = Password.GetNewPassword(password);
        var passwordBytes = Password.Prehash(password);
        var publicKey = new byte[Ed25519.PublicKeySize];
        var privateKey = GC.AllocateArray<byte>(Ed25519.PrivateKeySize, pinned: true);
        Ed25519.GenerateKeyPair(publicKey, privateKey);
        publicKey = Arrays.Concat(Constants.Ed25519KeyHeader, publicKey);
        Span<byte> encryptedPrivateKey = PrivateKey.Encrypt(passwordBytes, Constants.Ed25519KeyHeader, privateKey);
        return (Encodings.ToBase64(publicKey), Encodings.ToBase64(encryptedPrivateKey));
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
        string publicKeyFilePath = Path.ChangeExtension(privateKeyFilePath, Constants.PublicKeyExtension);
        try
        {
            if (File.Exists(publicKeyFilePath)) {
                return null;
            }
            CreateKeyFile(publicKeyFilePath, publicKey);
            return publicKeyFilePath;
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(publicKeyFilePath, ex.GetType().Name, "Unable to create a public key file.");
            return null;
        }
    }

    public static void CreateKeyFile(string filePath, string asymmetricKey)
    {
        if (File.Exists(filePath)) {
            File.SetAttributes(filePath, FileAttributes.Normal);
        }
        File.WriteAllText(filePath, asymmetricKey);
        File.SetAttributes(filePath, FileAttributes.ReadOnly);
    }

    public static Span<byte> GetCurve25519PublicKey(Span<byte> privateKey)
    {
        Span<byte> publicKey = stackalloc byte[X25519.PublicKeySize];
        X25519.ComputePublicKey(publicKey, privateKey);
        CryptographicOperations.ZeroMemory(privateKey);
        Span<byte> fullPublicKey = new byte[Constants.Curve25519KeyHeader.Length + publicKey.Length];
        Spans.Concat(fullPublicKey, Constants.Curve25519KeyHeader, publicKey);
        return fullPublicKey;
    }

    public static Span<byte> GetEd25519PublicKey(Span<byte> privateKey)
    {
        Span<byte> publicKey = stackalloc byte[Ed25519.PublicKeySize];
        Ed25519.ComputePublicKey(publicKey, privateKey);
        CryptographicOperations.ZeroMemory(privateKey);
        Span<byte> fullPublicKey = new byte[Constants.Ed25519KeyHeader.Length + publicKey.Length];
        Spans.Concat(fullPublicKey, Constants.Ed25519KeyHeader, publicKey);
        return fullPublicKey;
    }
}