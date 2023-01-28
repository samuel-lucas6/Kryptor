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
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Geralt;

namespace Kryptor;

public static class AsymmetricKeyValidation
{
    public static IEnumerable<string> GetGenerateKeyPairErrors(string directoryPath, int keyPairType, bool encryption, bool signing)
    {
        if (keyPairType != 1 && keyPairType != 2) {
            yield return "Invalid number.";
        }
        bool defaultKeyDirectory = string.Equals(directoryPath, Constants.DefaultKeyDirectory);
        if (!defaultKeyDirectory && !Directory.Exists(directoryPath)) {
            yield return ErrorMessages.GetFilePathError(directoryPath, ErrorMessages.DirectoryDoesNotExist);
        }
        if (encryption && signing) {
            yield return "Only specify one type of key pair to generate.";
        }
        else if (defaultKeyDirectory && !Globals.Overwrite) {
            if (keyPairType == 1 && (File.Exists(Constants.DefaultEncryptionPublicKeyPath) || File.Exists(Constants.DefaultEncryptionPrivateKeyPath))) {
                yield return "An encryption key pair already exists in the default directory. Specify -o|--overwrite if you want to overwrite your key pair.";
            }
            else if (keyPairType == 2 && (File.Exists(Constants.DefaultSigningPublicKeyPath) || File.Exists(Constants.DefaultSigningPrivateKeyPath))) {   
                yield return "A signing key pair already exists in the default directory. Specify -o|--overwrite if you want to overwrite your key pair.";
            }
        }
        else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 1) {
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PublicKeyExtension);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PrivateKeyExtension);
            if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath)) {
                yield return ErrorMessages.GetFilePathError(directoryPath, "An encryption key pair already exists in this directory. Specify -o|--overwrite if you want to overwrite your key pair.");
            }
        }
        else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 2) {
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PublicKeyExtension);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PrivateKeyExtension);
            if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath)) {
                yield return ErrorMessages.GetFilePathError(directoryPath, "A signing key pair already exists in this directory. Specify -o|--overwrite if you want to overwrite your key pair.");
            }
        }
    }

    public static IEnumerable<string> GetRecoverPublicKeyErrors(string privateKeyPath)
    {
        if (string.IsNullOrEmpty(privateKeyPath)) {
            yield return "Specify a private key file using -x:file.";
        }
        else if (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.InvalidPrivateKeyFile);
        }
        else if (!File.Exists(privateKeyPath)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.NonExistentPrivateKeyFile);
        }
        else if (new FileInfo(privateKeyPath).Length is not (Constants.EncryptionPrivateKeyLength or Constants.SigningPrivateKeyLength)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.InvalidPrivateKeyFileLength);
        }
    }
    
    public static List<byte[]> EncryptionPublicKeyFile(string[] publicKeyPaths)
    {
        try
        {
            var publicKeys = new List<byte[]>();
            foreach (string publicKeyPath in publicKeyPaths) {
                byte[] publicKey = GetPublicKeyFromFile(publicKeyPath);
                ValidateEncryptionKeyAlgorithm(publicKey);
                publicKey = publicKey[Constants.Curve25519KeyHeader.Length..];
                CheckIfDuplicate(publicKeys, publicKey);
                publicKeys.Add(publicKey);
            }
            return publicKeys;
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            if (ex is ArgumentException or IOException) {
                DisplayMessage.Exception(ex.GetType().Name, ex.Message);
                return null;
            }
            DisplayMessage.Exception(ex.GetType().Name, publicKeyPaths?.Length > 1 ? "One or more invalid encryption public keys." : "Invalid encryption public key.");
            return null;
        }
    }

    public static Span<byte> SigningPublicKeyFile(string publicKeyPath)
    {
        try
        {
            Span<byte> publicKey = GetPublicKeyFromFile(publicKeyPath);
            ValidateSigningKeyAlgorithm(publicKey);
            return publicKey[Constants.Ed25519KeyHeader.Length..];
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            DisplayMessage.FilePathException(publicKeyPath, ex.GetType().Name, ex.Message);
            return Span<byte>.Empty;
        }
    }
    
    private static byte[] GetPublicKeyFromFile(string publicKeyPath)
    {
        try
        {
            string encodedPublicKey = File.ReadAllText(publicKeyPath);
            return Encodings.FromBase64(encodedPublicKey);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            throw ex switch
            {
                FormatException => new FormatException(ex.Message, ex),
                _ => new IOException("Unable to read the public key file.", ex)
            };
        }
    }

    public static List<byte[]> EncryptionPublicKeyString(string[] encodedPublicKeys)
    {
        try
        {
            var publicKeys = new List<byte[]>();
            foreach (string encodedPublicKey in encodedPublicKeys) {
                byte[] publicKey = Encodings.FromBase64(encodedPublicKey);
                ValidateEncryptionKeyAlgorithm(publicKey);
                publicKey = publicKey[Constants.Curve25519KeyHeader.Length..];
                CheckIfDuplicate(publicKeys, publicKey);
                publicKeys.Add(publicKey);
            }
            return publicKeys;
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            if (ex is ArgumentException) {
                DisplayMessage.Exception(ex.GetType().Name, ex.Message);
                return null;
            }
            DisplayMessage.Exception(ex.GetType().Name, encodedPublicKeys?.Length > 1 ? "One or more invalid encryption public keys." : "Invalid encryption public key.");
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
            DisplayMessage.Exception(ex.GetType().Name, "Invalid signing public key.");
            return Span<byte>.Empty;
        }
    }

    private static void ValidateEncryptionKeyAlgorithm(Span<byte> asymmetricKey)
    {
        if (!ConstantTime.Equals(asymmetricKey[..Constants.Curve25519KeyHeader.Length], Constants.Curve25519KeyHeader)) {
            throw new NotSupportedException("Invalid key algorithm for encryption.");
        }
    }

    private static void CheckIfDuplicate(IEnumerable<byte[]> publicKeys, byte[] publicKey)
    {
        if (publicKeys.Any(key => key.SequenceEqual(publicKey))) {
            throw new ArgumentException("The same public key has been specified more than once.");
        }
    }

    private static void ValidateSigningKeyAlgorithm(Span<byte> asymmetricKey)
    {
        if (!ConstantTime.Equals(asymmetricKey[..Constants.Ed25519KeyHeader.Length], Constants.Ed25519KeyHeader)) {
            throw new NotSupportedException("Invalid key algorithm for signing.");
        }
    }

    public static Span<byte> EncryptionPrivateKeyFile(string privateKeyPath, Span<byte> passphrase)
    {
        try
        {
            Span<byte> privateKey = GetPrivateKeyFromFile(privateKeyPath);
            ValidateEncryptionKeyAlgorithm(privateKey);
            return DecryptPrivateKey(privateKey, passphrase);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            if (ex is CryptographicException) {
                DisplayMessage.Error(ex.Message);
                return Span<byte>.Empty;
            }
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, ex.Message);
            return Span<byte>.Empty;
        }
    }

    public static Span<byte> SigningPrivateKeyFile(string privateKeyPath, Span<byte> passphrase)
    {
        try
        {
            Span<byte> privateKey = GetPrivateKeyFromFile(privateKeyPath);
            ValidateSigningKeyAlgorithm(privateKey);
            return DecryptPrivateKey(privateKey, passphrase);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            if (ex is CryptographicException) {
                DisplayMessage.Error(ex.Message);
                return Span<byte>.Empty;
            }
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, ex.Message);
            return Span<byte>.Empty;
        }
    }

    public static Span<byte> GetPrivateKeyFromFile(string privateKeyPath)
    {
        try
        {
            string encodedPrivateKey = File.ReadAllText(privateKeyPath);
            return Encodings.FromBase64(encodedPrivateKey);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            throw ex switch
            {
                FormatException => new FormatException(ex.Message, ex),
                _ => new IOException("Unable to read the private key file.", ex)
            };
        }
    }

    public static Span<byte> DecryptPrivateKey(Span<byte> privateKey, Span<byte> passphrase)
    {
        Span<byte> keyVersion = privateKey.Slice(Constants.Curve25519KeyHeader.Length, Constants.PrivateKeyVersion.Length);
        if (!ConstantTime.Equals(keyVersion, Constants.PrivateKeyVersion)) {
            throw new NotSupportedException("This private key version isn't supported.");
        }
        if (passphrase.Length == 0) {
            passphrase = PassphrasePrompt.EnterYourPassphrase(isPrivateKey: true);
        }
        Console.WriteLine("Decrypting private key...");
        Console.WriteLine();
        return PrivateKey.Decrypt(privateKey, passphrase);
    }
}