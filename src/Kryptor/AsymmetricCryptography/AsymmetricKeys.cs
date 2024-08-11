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
using System.IO;
using Geralt;

namespace Kryptor;

public static class AsymmetricKeys
{
    public static (string publicKey, string privateKey) GenerateEncryptionKeyPair(Span<byte> passphrase)
    {
        passphrase = PassphrasePrompt.GetNewPassphrase(passphrase);

        Span<byte> publicKey = stackalloc byte[X25519.PublicKeySize];
        Span<byte> privateKey = stackalloc byte[X25519.PrivateKeySize];
        X25519.GenerateKeyPair(publicKey, privateKey);

        Span<byte> fullPublicKey = stackalloc byte[Constants.Curve25519KeyHeader.Length + publicKey.Length];
        Spans.Concat(fullPublicKey, Constants.Curve25519KeyHeader, publicKey);
        Span<byte> encryptedPrivateKey = PrivateKey.Encrypt(privateKey, passphrase, Constants.Curve25519KeyHeader);
        return (Encodings.ToBase64(fullPublicKey), Encodings.ToBase64(encryptedPrivateKey));
    }

    public static (string publicKey, string privateKey) GenerateSigningKeyPair(Span<byte> passphrase)
    {
        passphrase = PassphrasePrompt.GetNewPassphrase(passphrase);

        Span<byte> publicKey = stackalloc byte[Ed25519.PublicKeySize];
        Span<byte> privateKey = stackalloc byte[Ed25519.PrivateKeySize];
        Ed25519.GenerateKeyPair(publicKey, privateKey);

        Span<byte> fullPublicKey = stackalloc byte[Constants.Ed25519KeyHeader.Length + publicKey.Length];
        Spans.Concat(fullPublicKey, Constants.Ed25519KeyHeader, publicKey);
        Span<byte> encryptedPrivateKey = PrivateKey.Encrypt(privateKey, passphrase, Constants.Ed25519KeyHeader);
        return (Encodings.ToBase64(fullPublicKey), Encodings.ToBase64(encryptedPrivateKey));
    }

    public static (string publicKeyPath, string privateKeyPath) ExportKeyPair(string directoryPath, string fileName, string publicKey, string privateKey, string comment)
    {
        Directory.CreateDirectory(directoryPath);
        string publicKeyPath = Path.Combine(directoryPath, fileName + Constants.PublicKeyExtension);
        CreateKeyFile(publicKeyPath, publicKey, comment);
        string privateKeyPath = Path.Combine(directoryPath, fileName + Constants.PrivateKeyExtension);
        CreateKeyFile(privateKeyPath, privateKey, comment);
        return (publicKeyPath, privateKeyPath);
    }

    public static string ExportPublicKey(string privateKeyPath, string publicKey)
    {
        string publicKeyPath = Path.ChangeExtension(privateKeyPath, Constants.PublicKeyExtension);
        try
        {
            if (File.Exists(publicKeyPath)) {
                return null;
            }
            CreateKeyFile(publicKeyPath, publicKey, comment: string.Empty);
            return publicKeyPath;
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(publicKeyPath, ex.GetType().Name, "Unable to create a public key file.");
            return null;
        }
    }

    public static void CreateKeyFile(string filePath, string asymmetricKey, string comment)
    {
        if (File.Exists(filePath)) {
            File.SetAttributes(filePath, FileAttributes.Normal);
        }
        File.WriteAllText(filePath, string.IsNullOrWhiteSpace(comment) ? asymmetricKey : asymmetricKey + $" {comment}");
        File.SetAttributes(filePath, FileAttributes.ReadOnly);
    }

    public static string ReadKeyFileComment(string filePath, int stringKeyLength)
    {
        string comment = File.ReadAllText(filePath).TrimStart();
        return comment.Remove(startIndex: 0, stringKeyLength).TrimStart();
    }

    public static Span<byte> GetCurve25519PublicKey(Span<byte> privateKey)
    {
        Span<byte> publicKey = stackalloc byte[X25519.PublicKeySize];
        X25519.ComputePublicKey(publicKey, privateKey);

        Span<byte> fullPublicKey = new byte[Constants.Curve25519KeyHeader.Length + publicKey.Length];
        Spans.Concat(fullPublicKey, Constants.Curve25519KeyHeader, publicKey);
        return fullPublicKey;
    }

    public static Span<byte> GetEd25519PublicKey(Span<byte> privateKey)
    {
        Span<byte> publicKey = stackalloc byte[Ed25519.PublicKeySize];
        Ed25519.ComputePublicKey(publicKey, privateKey);

        Span<byte> fullPublicKey = new byte[Constants.Ed25519KeyHeader.Length + publicKey.Length];
        Spans.Concat(fullPublicKey, Constants.Ed25519KeyHeader, publicKey);
        return fullPublicKey;
    }
}
