﻿/*
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
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Geralt;
using System.IO;

namespace Kryptor;

public static class CommandLine
{
    public static void Encrypt(bool usePassphrase, Span<byte> passphrase, string symmetricKey, bool usePrivateKey, string privateKeyPath, string[] publicKeys, string[] filePaths)
    {
        if (!usePrivateKey && publicKeys == null && usePassphrase) {
            FileEncryptionWithPassphrase(passphrase, symmetricKey, filePaths);
        }
        else if (!usePrivateKey && publicKeys == null && !string.IsNullOrEmpty(symmetricKey)) {
            FileEncryptionWithSymmetricKey(symmetricKey, filePaths);
        }
        else if (publicKeys != null && !string.IsNullOrEmpty(privateKeyPath)) {
            if (publicKeys[0].EndsWith(Constants.PublicKeyExtension)) {
                FileEncryptionWithPublicKeyFile(privateKeyPath, passphrase, publicKeys, symmetricKey, filePaths);
                return;
            }
            FileEncryptionWithPublicKeyString(privateKeyPath, passphrase, publicKeys, symmetricKey, filePaths);
        }
        else if (!string.IsNullOrEmpty(privateKeyPath)) {
            FileEncryptionWithPrivateKey(privateKeyPath, passphrase, symmetricKey, filePaths);
        }
        else {
            DisplayMessage.Error(ErrorMessages.EncryptionMethod);
        }
    }

    private static void FileEncryptionWithPassphrase(Span<byte> passphrase, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetEncryptionErrors(symmetricKey).Concat(FileEncryptionValidation.GetEncryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        passphrase = PassphrasePrompt.GetNewPassphrase(passphrase);
        Span<byte> pepper = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithPassphrase(filePaths, passphrase, pepper);
    }
    
    private static void FileEncryptionWithSymmetricKey(string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetEncryptionErrors(symmetricKey).Concat(FileEncryptionValidation.GetEncryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> symmetricKeyBytes = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithSymmetricKey(filePaths, symmetricKeyBytes);
    }

    private static void FileEncryptionWithPublicKeyFile(string senderPrivateKeyPath, Span<byte> passphrase, string[] recipientPublicKeyPaths, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetEncryptionErrors(senderPrivateKeyPath, recipientPublicKeyPaths, symmetricKey).Concat(FileEncryptionValidation.GetEncryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath, passphrase);
        List<byte[]> recipientPublicKeys = AsymmetricKeyValidation.EncryptionPublicKeyFile(recipientPublicKeyPaths);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithPublicKey(filePaths, senderPrivateKey, recipientPublicKeys, preSharedKey);
    }

    private static void FileEncryptionWithPublicKeyString(string senderPrivateKeyPath, Span<byte> passphrase, string[] recipientPublicKeyStrings, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetEncryptionErrors(senderPrivateKeyPath, recipientPublicKeyStrings, symmetricKey).Concat(FileEncryptionValidation.GetEncryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath, passphrase);
        List<byte[]> recipientPublicKeys = AsymmetricKeyValidation.EncryptionPublicKeyString(recipientPublicKeyStrings);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithPublicKey(filePaths, senderPrivateKey, recipientPublicKeys, preSharedKey);
    }

    private static void FileEncryptionWithPrivateKey(string privateKeyPath, Span<byte> passphrase, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetEncryptionErrors(privateKeyPath, symmetricKey).Concat(FileEncryptionValidation.GetEncryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath, passphrase);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithPrivateKey(filePaths, privateKey, preSharedKey);
    }

    public static void Decrypt(bool usePassphrase, Span<byte> passphrase, string symmetricKey, bool usePrivateKey, string privateKeyPath, string[] publicKeys, string[] filePaths)
    {
        if (!usePrivateKey && publicKeys == null && usePassphrase) {
            FileDecryptionWithPassphrase(passphrase, symmetricKey, filePaths);
        }
        else if (!usePrivateKey && publicKeys == null && !string.IsNullOrEmpty(symmetricKey)) {
            FileDecryptionWithSymmetricKey(symmetricKey, filePaths);
        }
        else if (publicKeys != null && !string.IsNullOrEmpty(privateKeyPath)) {
            if (publicKeys[0].EndsWith(Constants.PublicKeyExtension)) {
                FileDecryptionWithPublicKeyFile(privateKeyPath, passphrase, publicKeys, symmetricKey, filePaths);
                return;
            }
            FileDecryptionWithPublicKeyString(privateKeyPath, passphrase, publicKeys, symmetricKey, filePaths);
        }
        else if (!string.IsNullOrEmpty(privateKeyPath)) {
            FileDecryptionWithPrivateKey(privateKeyPath, passphrase, symmetricKey, filePaths);
        }
        else {
            DisplayMessage.Error(ErrorMessages.EncryptionMethod);
        }
    }

    private static void FileDecryptionWithPassphrase(Span<byte> passphrase, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetDecryptionErrors(symmetricKey).Concat(FileEncryptionValidation.GetDecryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        if (passphrase.Length == 0) {
            passphrase = PassphrasePrompt.EnterYourPassphrase();
        }
        Span<byte> pepper = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithPassphrase(filePaths, passphrase, pepper);
    }
    
    private static void FileDecryptionWithSymmetricKey(string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetDecryptionErrors(symmetricKey).Concat(FileEncryptionValidation.GetDecryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> symmetricKeyBytes = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithSymmetricKey(filePaths, symmetricKeyBytes);
    }

    private static void FileDecryptionWithPublicKeyFile(string recipientPrivateKeyPath, Span<byte> passphrase, string[] senderPublicKeyPaths, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetDecryptionErrors(recipientPrivateKeyPath, senderPublicKeyPaths, symmetricKey).Concat(FileEncryptionValidation.GetDecryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath, passphrase);
        List<byte[]> senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyFile(senderPublicKeyPaths);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithPublicKey(filePaths, recipientPrivateKey, senderPublicKey?[0] ?? Span<byte>.Empty, preSharedKey);
    }

    private static void FileDecryptionWithPublicKeyString(string recipientPrivateKeyPath, Span<byte> passphrase, string[] senderPublicKeyStrings, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetDecryptionErrors(recipientPrivateKeyPath, senderPublicKeyStrings, symmetricKey).Concat(FileEncryptionValidation.GetDecryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath, passphrase);
        List<byte[]> senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyString(senderPublicKeyStrings);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithPublicKey(filePaths, recipientPrivateKey, senderPublicKey?[0] ?? Span<byte>.Empty, preSharedKey);
    }

    private static void FileDecryptionWithPrivateKey(string privateKeyPath, Span<byte> passphrase, string symmetricKey, string[] filePaths)
    {
        IEnumerable<string> errorMessages = FileEncryptionValidation.GetDecryptionErrors(privateKeyPath, symmetricKey).Concat(FileEncryptionValidation.GetDecryptionFilePathErrors(filePaths));
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath, passphrase);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithPrivateKey(filePaths, privateKey, preSharedKey);
    }

    public static void GenerateNewKeyPair(string directoryPath, Span<byte> passphrase, string comment, bool encryption, bool signing)
    {
        try
        {
            int keyPairType = GetKeyPairType(encryption, signing);
            IEnumerable<string> errorMessages = AsymmetricKeyValidation.GetGenerateKeyPairErrors(directoryPath, keyPairType, comment, encryption, signing);
            DisplayMessage.AllErrors(errorMessages);
            string publicKey, privateKey, publicKeyPath, privateKeyPath;
            if (keyPairType == 1) {
                (publicKey, privateKey) = AsymmetricKeys.GenerateEncryptionKeyPair(passphrase);
                (publicKeyPath, privateKeyPath) = AsymmetricKeys.ExportKeyPair(directoryPath, Constants.DefaultEncryptionKeyFileName, publicKey, privateKey, comment);
            }
            else {
                (publicKey, privateKey) = AsymmetricKeys.GenerateSigningKeyPair(passphrase);
                (publicKeyPath, privateKeyPath) = AsymmetricKeys.ExportKeyPair(directoryPath, Constants.DefaultSigningKeyFileName, publicKey, privateKey, comment);
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
        if (!string.IsNullOrEmpty(userInput)) {
            Console.WriteLine();
        }
        _ = int.TryParse(userInput, out int keyPairType);
        return keyPairType;
    }

    public static void RecoverPublicKey(string privateKeyPath, Span<byte> passphrase)
    {
        try
        {
            IEnumerable<string> errorMessages = AsymmetricKeyValidation.GetRecoverPublicKeyErrors(privateKeyPath);
            DisplayMessage.AllErrors(errorMessages);
            Span<byte> privateKey = AsymmetricKeyValidation.GetPrivateKeyFromFile(privateKeyPath);
            privateKey = AsymmetricKeyValidation.DecryptPrivateKey(privateKey, passphrase);
            
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

    public static void Sign(string privateKeyPath, Span<byte> passphrase, string comment, bool prehash, string[] signaturePaths, string[] filePaths)
    {
        IEnumerable<string> errorMessages = SigningValidation.GetSignErrors(privateKeyPath, comment, signaturePaths, filePaths);
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> privateKey = AsymmetricKeyValidation.SigningPrivateKeyFile(privateKeyPath, passphrase);
        FileSigning.SignEachFile(filePaths, signaturePaths, comment, prehash, privateKey);
    }

    public static void Verify(string[] publicKeys, string[] signaturePaths, string[] filePaths)
    {
        IEnumerable<string> errorMessages = SigningValidation.GetVerifyErrors(publicKeys, signaturePaths, filePaths);
        DisplayMessage.AllErrors(errorMessages);
        Span<byte> publicKey = publicKeys[0].EndsWith(Constants.PublicKeyExtension) ? AsymmetricKeyValidation.SigningPublicKeyFile(publicKeys[0]) : AsymmetricKeyValidation.SigningPublicKeyString(publicKeys[0]);
        FileSigning.VerifyEachFile(signaturePaths, filePaths, publicKey);
    }
    
    public static void CheckForUpdates()
    {
        if (isAURPackage())
        {
            DisplayMessage.WriteLine("Kryptor is installed through AUR so it can't be updated manually.", ConsoleColor.Red);
            Environment.Exit(-1);
        }
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
                    DisplayMessage.Error("You didn't type y or n.");
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

    /// Check if this kryptor binary is installed through AUR, if true we cannot update the binary manually 
    private static bool isAURPackage()
    {
        string executableFilePath = Environment.ProcessPath ?? throw new FileNotFoundException("Unable to retrieve the process path.");
        var isArchBased = File.Exists("/usr/bin/pacman");
        var isKryptorInstalledUsingAUR = String.Equals("/usr/bin/kryptor", executableFilePath);
        return isArchBased && isKryptorInstalledUsingAUR;
    }
}