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
            yield return "Please enter a valid number.";
        }
        bool defaultKeyDirectory = string.Equals(directoryPath, Constants.DefaultKeyDirectory);
        if (!defaultKeyDirectory && !Directory.Exists(directoryPath)) {
            yield return ErrorMessages.GetFilePathError(directoryPath, "This directory doesn't exist.");
        }
        if (encryption && signing) {
            yield return "Please specify only one type of key pair to generate.";
        }
        else if (defaultKeyDirectory && !Globals.Overwrite) {
            if (keyPairType == 1 && (File.Exists(Constants.DefaultEncryptionPublicKeyPath) || File.Exists(Constants.DefaultEncryptionPrivateKeyPath))) {
                yield return "An encryption key pair already exists in the default directory. Please specify -o|--overwrite if you want to overwrite your key pair.";
            }
            else if (keyPairType == 2 && (File.Exists(Constants.DefaultSigningPublicKeyPath) || File.Exists(Constants.DefaultSigningPrivateKeyPath))) {   
                yield return "A signing key pair already exists in the default directory. Please specify -o|--overwrite if you want to overwrite your key pair.";
            }
        }
        else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 1) {
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PublicKeyExtension);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PrivateKeyExtension);
            if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath)) {
                yield return ErrorMessages.GetFilePathError(directoryPath, "An encryption key pair already exists in this directory. Please specify -o|--overwrite if you want to overwrite your key pair.");
            }
        }
        else if (!defaultKeyDirectory && !Globals.Overwrite && keyPairType == 2) {
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PublicKeyExtension);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PrivateKeyExtension);
            if (File.Exists(publicKeyPath) || File.Exists(privateKeyPath)) {
                yield return ErrorMessages.GetFilePathError(directoryPath, "A signing key pair already exists in this directory. Please specify -o|--overwrite if you want to overwrite your key pair.");
            }
        }
    }

    public static IEnumerable<string> GetRecoverPublicKeyErrors(string privateKeyPath)
    {
        if (string.IsNullOrEmpty(privateKeyPath)) {
            yield return "Please specify a private key file using -x:file.";
        }
        else if (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.InvalidPrivateKeyFile);
        }
        else if (!File.Exists(privateKeyPath)) {
            yield return ErrorMessages.GetFilePathError(privateKeyPath, ErrorMessages.NonExistentPrivateKeyFile);
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
            if (ex is ArgumentException) {
                DisplayMessage.Error(ex.Message);
                return null;
            }
            DisplayMessage.Exception(ex.GetType().Name, publicKeyPaths?.Length > 1 ? "Please specify valid encryption public keys." : "Please specify a valid encryption public key.");
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
            if (encodedPublicKey.Length != Constants.PublicKeyLength) {
                throw new ArgumentException("Please specify a valid public key file.");
            }
            return Encodings.FromBase64(encodedPublicKey);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            if (ex is ArgumentException) { throw new ArgumentException(ex.Message, ex); }
            if (ex is FormatException) { throw new FormatException(ex.Message, ex); }
            throw new IOException("Unable to read the public key file.", ex);
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
            DisplayMessage.Exception(ex.GetType().Name, ex.Message);
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
            return Span<byte>.Empty;
        }
    }

    private static void ValidateEncryptionKeyAlgorithm(Span<byte> asymmetricKey)
    {
        Span<byte> keyAlgorithm = asymmetricKey[..Constants.Curve25519KeyHeader.Length];
        if (!ConstantTime.Equals(keyAlgorithm, Constants.Curve25519KeyHeader) && !ConstantTime.Equals(keyAlgorithm, Constants.OldCurve25519KeyHeader)) {
            throw new NotSupportedException("This key algorithm isn't supported for encryption.");
        }
    }

    private static void CheckIfDuplicate(IEnumerable<byte[]> publicKeys, byte[] publicKey)
    {
        if (publicKeys.Any(key => key.SequenceEqual(publicKey))) {
            throw new NotSupportedException("The same public key has been specified more than once.");
        }
    }

    private static void ValidateSigningKeyAlgorithm(Span<byte> asymmetricKey)
    {
        Span<byte> keyAlgorithm = asymmetricKey[..Constants.Ed25519KeyHeader.Length];
        if (!ConstantTime.Equals(keyAlgorithm, Constants.Ed25519KeyHeader) && !ConstantTime.Equals(keyAlgorithm, Constants.OldEd25519KeyHeader)) {
            throw new NotSupportedException("This key algorithm isn't supported for signing.");
        }
    }

    public static Span<byte> EncryptionPrivateKeyFile(string privateKeyPath, Span<byte> password)
    {
        try
        {
            Span<byte> privateKey = GetPrivateKeyFromFile(privateKeyPath);
            ValidateEncryptionKeyAlgorithm(privateKey);
            return DecryptPrivateKey(privateKey, password, privateKeyPath);
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

    public static Span<byte> SigningPrivateKeyFile(string privateKeyPath, Span<byte> password)
    {
        try
        {
            Span<byte> privateKey = GetPrivateKeyFromFile(privateKeyPath);
            ValidateSigningKeyAlgorithm(privateKey);
            return DecryptPrivateKey(privateKey, password, privateKeyPath);
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
            if (encodedPrivateKey.Length != Constants.V2EncryptionPrivateKeyLength && encodedPrivateKey.Length != Constants.V2SigningPrivateKeyLength && encodedPrivateKey.Length != Constants.V1EncryptionPrivateKeyLength && encodedPrivateKey.Length != Constants.V1SigningPrivateKeyLength) {
                throw new ArgumentException("Please specify a valid private key file.");
            }
            return Encodings.FromBase64(encodedPrivateKey);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            if (ex is ArgumentException) { throw new ArgumentException(ex.Message, ex); }
            if (ex is FormatException) { throw new FormatException(ex.Message, ex); }
            throw new IOException("Unable to read the private key file.", ex);
        }
    }

    public static Span<byte> DecryptPrivateKey(Span<byte> privateKey, Span<byte> password, string privateKeyPath)
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
        Console.WriteLine();
        if (version2) {
            return PrivateKey.DecryptV2(privateKey, password);
        }

        Span<byte> prehashedPassword = stackalloc byte[BLAKE2b.MaxHashSize];
        BLAKE2b.ComputeHash(prehashedPassword, password);
        Span<byte> decryptedPrivateKey = PrivateKey.DecryptV1(privateKey, prehashedPassword);
        
        Span<byte> plaintextPrivateKey = GC.AllocateArray<byte>(decryptedPrivateKey.Length, pinned: true);
        decryptedPrivateKey.CopyTo(plaintextPrivateKey);
        try
        {
            Console.WriteLine("Updating private key format...");
            Span<byte> keyAlgorithm = privateKey[..Constants.KeyAlgorithmLength];
            Span<byte> v2PrivateKey = PrivateKey.Encrypt(decryptedPrivateKey, password, keyAlgorithm);
            AsymmetricKeys.CreateKeyFile(privateKeyPath, Encodings.ToBase64(v2PrivateKey));
            Console.WriteLine("Private key format successfully updated.");
            Console.WriteLine();
            
            Console.WriteLine("Updating public key format...");
            Span<byte> publicKey = plaintextPrivateKey.Length switch
            {
                X25519.PrivateKeySize => AsymmetricKeys.GetCurve25519PublicKey(plaintextPrivateKey),
                _ => AsymmetricKeys.GetEd25519PublicKey(plaintextPrivateKey)
            };
            string publicKeyString = Encodings.ToBase64(publicKey);
            string publicKeyPath = Path.ChangeExtension(privateKeyPath, Constants.PublicKeyExtension);
            AsymmetricKeys.CreateKeyFile(publicKeyPath, publicKeyString);
            DisplayMessage.PublicKey(publicKeyString, publicKeyPath);
            Console.WriteLine();
            DisplayMessage.WriteLine("IMPORTANT: Please back up these updated files to external storage (e.g. memory sticks).", ConsoleColor.DarkYellow);
            Console.WriteLine();
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, "Unable to update private and/or public key to latest format.");
        }
        return plaintextPrivateKey;
    }
}