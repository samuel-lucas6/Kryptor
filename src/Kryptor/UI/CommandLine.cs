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
using System.Net;
using System.Security.Cryptography;
using Geralt;

namespace Kryptor;

public static class CommandLine
{
    public static void Encrypt(bool usePassword, Span<byte> password, string symmetricKey, bool usePrivateKey, string privateKeyPath, string[] publicKeys, string[] filePaths)
    {
        if (!usePrivateKey && publicKeys == null && usePassword) {
            FileEncryptionWithPassword(password, symmetricKey, filePaths);
        }
        else if (!usePrivateKey && publicKeys == null && !string.IsNullOrEmpty(symmetricKey)) {
            FileEncryptionWithSymmetricKey(symmetricKey, filePaths);
        }
        else if (publicKeys != null && !string.IsNullOrEmpty(privateKeyPath)) {
            if (publicKeys[0].EndsWith(Constants.PublicKeyExtension)) {
                FileEncryptionWithPublicKeyFile(privateKeyPath, password, publicKeys, symmetricKey, filePaths);
                return;
            }
            FileEncryptionWithPublicKeyString(privateKeyPath, password, publicKeys, symmetricKey, filePaths);
        }
        else if (!string.IsNullOrEmpty(privateKeyPath)) {
            FileEncryptionWithPrivateKey(privateKeyPath, password, symmetricKey, filePaths);
        }
        else {
            DisplayMessage.Error(ErrorMessages.EncryptionMethod);
        }
    }

    private static void FileEncryptionWithPassword(Span<byte> password, string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileEncryptionWithPassword(usePassword: true, symmetricKey, filePaths);
        password = PasswordPrompt.GetNewPassword(password);
        Span<byte> pepper = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithPassword(filePaths, password, pepper);
    }
    
    private static void FileEncryptionWithSymmetricKey(string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileEncryptionWithPassword(usePassword: false, symmetricKey, filePaths);
        Span<byte> symmetricKeyBytes = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithSymmetricKey(filePaths, symmetricKeyBytes);
    }

    private static void FileEncryptionWithPublicKeyFile(string senderPrivateKeyPath, Span<byte> password, string[] recipientPublicKeyPaths, string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileEncryptionWithPublicKeyFile(senderPrivateKeyPath, recipientPublicKeyPaths, symmetricKey, filePaths);
        Span<byte> senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath, password);
        List<byte[]> recipientPublicKeys = AsymmetricKeyValidation.EncryptionPublicKeyFile(recipientPublicKeyPaths);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithPublicKey(senderPrivateKey, recipientPublicKeys, preSharedKey, filePaths);
    }

    private static void FileEncryptionWithPublicKeyString(string senderPrivateKeyPath, Span<byte> password, string[] recipientPublicKeyStrings, string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileEncryptionWithPublicKeyString(senderPrivateKeyPath, recipientPublicKeyStrings, symmetricKey, filePaths);
        Span<byte> senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath, password);
        List<byte[]> recipientPublicKeys = AsymmetricKeyValidation.EncryptionPublicKeyString(recipientPublicKeyStrings);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithPublicKey(senderPrivateKey, recipientPublicKeys, preSharedKey, filePaths);
    }

    private static void FileEncryptionWithPrivateKey(string privateKeyPath, Span<byte> password, string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileEncryptionWithPrivateKey(privateKeyPath, symmetricKey, filePaths);
        Span<byte> privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath, password);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithPrivateKey(privateKey, preSharedKey, filePaths);
    }

    public static void Decrypt(bool usePassword, Span<byte> password, string symmetricKey, bool usePrivateKey, string privateKeyPath, string[] publicKeys, string[] filePaths)
    {
        if (!usePrivateKey && publicKeys == null && usePassword) {
            FileDecryptionWithPassword(password, symmetricKey, filePaths);
        }
        else if (!usePrivateKey && publicKeys == null && !string.IsNullOrEmpty(symmetricKey)) {
            FileDecryptionWithSymmetricKey(symmetricKey, filePaths);
        }
        else if (publicKeys != null && !string.IsNullOrEmpty(privateKeyPath)) {
            if (publicKeys[0].EndsWith(Constants.PublicKeyExtension)) {
                FileDecryptionWithPublicKeyFile(privateKeyPath, password, publicKeys, symmetricKey, filePaths);
                return;
            }
            FileDecryptionWithPublicKeyString(privateKeyPath, password, publicKeys, symmetricKey, filePaths);
        }
        else if (!string.IsNullOrEmpty(privateKeyPath)) {
            FileDecryptionWithPrivateKey(privateKeyPath, password, symmetricKey, filePaths);
        }
        else {
            DisplayMessage.Error(ErrorMessages.EncryptionMethod);
        }
    }

    private static void FileDecryptionWithPassword(Span<byte> password, string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileDecryptionWithPassword(usePassword: true, symmetricKey, filePaths);
        if (password.Length == 0) {
            password = PasswordPrompt.EnterYourPassword();
        }
        Span<byte> pepper = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithPassword(filePaths, password, pepper);
    }
    
    private static void FileDecryptionWithSymmetricKey(string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileDecryptionWithPassword(usePassword: false, symmetricKey, filePaths);
        Span<byte> symmetricKeyBytes = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithSymmetricKey(filePaths, symmetricKeyBytes);
    }

    private static void FileDecryptionWithPublicKeyFile(string recipientPrivateKeyPath, Span<byte> password, string[] senderPublicKeyPaths, string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileDecryptionWithPublicKeyFile(recipientPrivateKeyPath, senderPublicKeyPaths, symmetricKey, filePaths);
        Span<byte> recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath, password);
        List<byte[]> senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyFile(senderPublicKeyPaths);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithPublicKey(recipientPrivateKey, senderPublicKey?[0], preSharedKey, filePaths);
    }

    private static void FileDecryptionWithPublicKeyString(string recipientPrivateKeyPath, Span<byte> password, string[] senderPublicKeyStrings, string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileDecryptionWithPublicKeyString(recipientPrivateKeyPath, senderPublicKeyStrings, symmetricKey, filePaths);
        Span<byte> recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath, password);
        List<byte[]> senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyString(senderPublicKeyStrings);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithPublicKey(recipientPrivateKey, senderPublicKey?[0], preSharedKey, filePaths);
    }

    private static void FileDecryptionWithPrivateKey(string privateKeyPath, Span<byte> password, string symmetricKey, string[] filePaths)
    {
        FileEncryptionValidation.FileDecryptionWithPrivateKey(privateKeyPath, symmetricKey, filePaths);
        Span<byte> privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath, password);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithPrivateKey(privateKey, preSharedKey, filePaths);
    }

    public static void GenerateNewKeyPair(string directoryPath, Span<byte> password, bool encryption, bool signing)
    {
        try
        {
            int keyPairType = GetKeyPairType(encryption, signing);
            FilePathValidation.GenerateKeyPair(directoryPath, keyPairType, encryption, signing);
            string publicKey, privateKey, publicKeyPath, privateKeyPath;
            if (keyPairType == 1) {
                (publicKey, privateKey) = AsymmetricKeys.GenerateEncryptionKeyPair(password);
                (publicKeyPath, privateKeyPath) = AsymmetricKeys.ExportKeyPair(directoryPath, Constants.DefaultEncryptionKeyFileName, publicKey, privateKey);
            }
            else {
                (publicKey, privateKey) = AsymmetricKeys.GenerateSigningKeyPair(password);
                (publicKeyPath, privateKeyPath) = AsymmetricKeys.ExportKeyPair(directoryPath, Constants.DefaultSigningKeyFileName, publicKey, privateKey);
            }
            DisplayMessage.KeyPair(publicKey, publicKeyPath, privateKeyPath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to create the key pair files.");
        }
    }

    private static int GetKeyPairType(bool encryption, bool signing)
    {
        if (encryption) { return 1; }
        if (signing) { return 2; }
        Console.WriteLine("Please select a key pair type (type 1 or 2):");
        Console.WriteLine("1) Encryption");
        Console.WriteLine("2) Signing");
        string userInput = Console.ReadLine();
        Console.WriteLine();
        _ = int.TryParse(userInput, out int keyPairType);
        return keyPairType;
    }

    public static void RecoverPublicKey(string privateKeyPath, Span<byte> password)
    {
        try
        {
            FilePathValidation.RecoverPublicKey(privateKeyPath);
            Span<byte> privateKey = AsymmetricKeyValidation.GetPrivateKeyFromFile(privateKeyPath);
            privateKey = AsymmetricKeyValidation.DecryptPrivateKey(privateKey, password, privateKeyPath);
            
            Span<byte> publicKey = privateKey.Length switch
            {
                X25519.PrivateKeySize => AsymmetricKeys.GetCurve25519PublicKey(privateKey),
                _ => AsymmetricKeys.GetEd25519PublicKey(privateKey)
            };
            CryptographicOperations.ZeroMemory(privateKey);
            
            string publicKeyString = Encodings.ToBase64(publicKey);
            string publicKeyPath = AsymmetricKeys.ExportPublicKey(privateKeyPath, publicKeyString);
            DisplayMessage.PublicKey(publicKeyString, publicKeyPath);
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            if (ex is CryptographicException) {
                DisplayMessage.Error(ex.Message);
                return;
            }
            DisplayMessage.FilePathException(privateKeyPath, ex.GetType().Name, ex.Message);
        }
    }

    public static void Sign(string privateKeyPath, Span<byte> password, string comment, bool prehash, string[] signatureFilePaths, string[] filePaths)
    {
        SigningValidation.Sign(privateKeyPath, comment, signatureFilePaths, filePaths);
        Span<byte> privateKey = AsymmetricKeyValidation.SigningPrivateKeyFile(privateKeyPath, password);
        FileSigning.SignEachFile(filePaths, signatureFilePaths, comment, prehash, privateKey);
    }

    public static void Verify(string[] publicKeys, string[] signatureFilePaths, string[] filePaths)
    {
        Span<byte> publicKey;
        if (publicKeys == null || publicKeys[0].EndsWith(Constants.PublicKeyExtension)) {
            SigningValidation.VerifyWithPublicKeyFile(publicKeys, signatureFilePaths, filePaths);
            publicKey = AsymmetricKeyValidation.SigningPublicKeyFile(publicKeys[0]);
        }
        else {
            SigningValidation.VerifyWithPublicKeyString(publicKeys, signatureFilePaths, filePaths);
            publicKey = AsymmetricKeyValidation.SigningPublicKeyString(publicKeys[0]);
        }
        FileSigning.VerifyEachFile(signatureFilePaths, filePaths, publicKey);
    }
    
    public static void CheckForUpdates()
    {
        try
        {
            bool updateAvailable = Updates.CheckForUpdates(out string latestVersion);
            if (!updateAvailable) {
                DisplayMessage.WriteLine("Kryptor is up to date.", ConsoleColor.Green);
                return;
            }
            Console.WriteLine($"An update is available for Kryptor. The latest version is v{latestVersion}.");
            Console.WriteLine();
            DisplayMessage.WriteLine($"IMPORTANT: Please check the latest changelog at <https://www.kryptor.co.uk/changelog#v{latestVersion}> to see if there are any breaking changes BEFORE updating.", ConsoleColor.DarkYellow);
            Console.WriteLine();
            Console.WriteLine("Would you like Kryptor to automatically install this update now? (type y or n)");
            string userInput = Console.ReadLine()?.ToLower();
            if (!string.IsNullOrEmpty(userInput)) {
                Console.WriteLine();
            }
            switch (userInput) {
                case "y":
                    Updates.Update(latestVersion);
                    break;
                case "n":
                    Console.WriteLine("Please specify -u|--update again when you're ready to update.");
                    Console.WriteLine();
                    Console.WriteLine("Alternatively, you can manually download the latest release at <https://www.kryptor.co.uk/#download-kryptor>.");
                    return;
                default:
                    DisplayMessage.Error("Please type either y or n next time.");
                    return;
            }
        }
        catch (Exception ex) when (ex is WebException or PlatformNotSupportedException or FormatException or OverflowException or InvalidOperationException || ExceptionFilters.Cryptography(ex))
        {
            if (ex is PlatformNotSupportedException or CryptographicException) {
                DisplayMessage.Exception(ex.GetType().Name, ex.Message);
                return;
            }
            DisplayMessage.Exception(ex.GetType().Name, "Unable to check for updates, or the download failed.");
            Console.WriteLine();
            Console.WriteLine("You can manually download the latest release at <https://www.kryptor.co.uk/#download-kryptor>.");
        }
    }
}