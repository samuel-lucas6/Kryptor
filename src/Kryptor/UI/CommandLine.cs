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
    public static void Encrypt(bool usePassword, char[] password, string symmetricKey, bool usePrivateKey, string privateKeyPath, string[] publicKeys, string[] filePaths)
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

    private static void FileEncryptionWithPassword(char[] password, string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPassword(usePassword: true, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        password = Password.GetNewPassword(password);
        Span<byte> pepper = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        Span<byte> passwordBytes = !string.IsNullOrEmpty(symmetricKey) && pepper == default ? default : Password.Prehash(password, pepper);
        FileEncryption.EncryptEachFileWithPassword(filePaths, passwordBytes);
    }
    
    private static void FileEncryptionWithSymmetricKey(string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPassword(usePassword: false, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> symmetricKeyBytes = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        FileEncryption.EncryptEachFileWithSymmetricKey(filePaths, symmetricKeyBytes);
    }

    private static void FileEncryptionWithPublicKeyFile(string senderPrivateKeyPath, char[] password, string[] recipientPublicKeyPaths, string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPublicKeyFile(senderPrivateKeyPath, recipientPublicKeyPaths, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath, password);
        List<byte[]> recipientPublicKeys = AsymmetricKeyValidation.EncryptionPublicKeyFile(recipientPublicKeyPaths);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        if (!string.IsNullOrEmpty(symmetricKey) && preSharedKey == default) {
            return;
        }
        FileEncryption.EncryptEachFileWithPublicKey(senderPrivateKey, recipientPublicKeys, preSharedKey, filePaths);
    }

    private static void FileEncryptionWithPublicKeyString(string senderPrivateKeyPath, char[] password, string[] recipientPublicKeyStrings, string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPublicKeyString(senderPrivateKeyPath, recipientPublicKeyStrings, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath, password);
        List<byte[]> recipientPublicKeys = AsymmetricKeyValidation.EncryptionPublicKeyString(recipientPublicKeyStrings);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        if (!string.IsNullOrEmpty(symmetricKey) && preSharedKey == default) {
            return;
        }
        FileEncryption.EncryptEachFileWithPublicKey(senderPrivateKey, recipientPublicKeys, preSharedKey, filePaths);
    }

    private static void FileEncryptionWithPrivateKey(string privateKeyPath, char[] password, string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPrivateKey(privateKeyPath, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath, password);
        Span<byte> preSharedKey = SymmetricKeyValidation.GetEncryptionSymmetricKey(symmetricKey);
        if (!string.IsNullOrEmpty(symmetricKey) && preSharedKey == default) {
            return;
        }
        FileEncryption.EncryptEachFileWithPrivateKey(privateKey, preSharedKey, filePaths);
    }

    public static void Decrypt(bool usePassword, char[] password, string symmetricKey, bool usePrivateKey, string privateKeyPath, string[] publicKeys, string[] filePaths)
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

    private static void FileDecryptionWithPassword(char[] password, string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPassword(usePassword: true, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        if (password.Length == 0) {
            password = PasswordPrompt.EnterYourPassword();
        }
        Span<byte> pepper = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        Span<byte> passwordBytes = !string.IsNullOrEmpty(symmetricKey) && pepper == default ? default : Password.Prehash(password, pepper);
        FileDecryption.DecryptEachFileWithPassword(filePaths, passwordBytes);
    }
    
    private static void FileDecryptionWithSymmetricKey(string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPassword(usePassword: false, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> symmetricKeyBytes = SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey);
        FileDecryption.DecryptEachFileWithSymmetricKey(filePaths, symmetricKeyBytes);
    }

    private static void FileDecryptionWithPublicKeyFile(string recipientPrivateKeyPath, char[] password, string[] senderPublicKeyPaths, string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPublicKeyFile(recipientPrivateKeyPath, senderPublicKeyPaths, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath, password);
        List<byte[]> senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyFile(senderPublicKeyPaths);
        Span<byte> preSharedKey = !string.IsNullOrEmpty(symmetricKey) ? SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey) : default;
        if (!string.IsNullOrEmpty(symmetricKey) && preSharedKey == default) {
            return;
        }
        FileDecryption.DecryptEachFileWithPublicKey(recipientPrivateKey, senderPublicKey?[0], preSharedKey, filePaths);
    }

    private static void FileDecryptionWithPublicKeyString(string recipientPrivateKeyPath, char[] password, string[] senderPublicKeyStrings, string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPublicKeyString(recipientPrivateKeyPath, senderPublicKeyStrings, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath, password);
        List<byte[]> senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyString(senderPublicKeyStrings);
        Span<byte> preSharedKey = !string.IsNullOrEmpty(symmetricKey) ? SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey) : default;
        if (!string.IsNullOrEmpty(symmetricKey) && preSharedKey == default) {
            return;
        }
        FileDecryption.DecryptEachFileWithPublicKey(recipientPrivateKey, senderPublicKey?[0], preSharedKey, filePaths);
    }

    private static void FileDecryptionWithPrivateKey(string privateKeyPath, char[] password, string symmetricKey, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPrivateKey(privateKeyPath, symmetricKey, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath, password);
        Span<byte> preSharedKey = !string.IsNullOrEmpty(symmetricKey) ? SymmetricKeyValidation.GetDecryptionSymmetricKey(symmetricKey) : default;
        if (!string.IsNullOrEmpty(symmetricKey) && preSharedKey == default) {
            return;
        }
        FileDecryption.DecryptEachFileWithPrivateKey(privateKey, preSharedKey, filePaths);
    }

    public static void GenerateNewKeyPair(char[] password, string exportDirectoryPath, bool encryption, bool signing)
    {
        try
        {
            int keyPairType = GetKeyPairType(encryption, signing);
            bool validUserInput = FilePathValidation.GenerateKeyPair(exportDirectoryPath, keyPairType, encryption, signing);
            if (!validUserInput) {
                return;
            }
            string publicKey, privateKey, publicKeyFilePath, privateKeyFilePath;
            if (keyPairType == 1) {
                (publicKey, privateKey) = AsymmetricKeys.GenerateEncryptionKeyPair(password);
                (publicKeyFilePath, privateKeyFilePath) = AsymmetricKeys.ExportKeyPair(exportDirectoryPath, Constants.DefaultEncryptionKeyFileName, publicKey, privateKey);
            }
            else {
                (publicKey, privateKey) = AsymmetricKeys.GenerateSigningKeyPair(password);
                (publicKeyFilePath, privateKeyFilePath) = AsymmetricKeys.ExportKeyPair(exportDirectoryPath, Constants.DefaultSigningKeyFileName, publicKey, privateKey);
            }
            DisplayMessage.KeyPair(publicKey, publicKeyFilePath, privateKeyFilePath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(exportDirectoryPath, ex.GetType().Name, "Unable to create the key pair files.");
        }
    }

    private static int GetKeyPairType(bool encryption, bool signing)
    {
        if (encryption) {
            return 1;
        }
        if (signing) {
            return 2;
        }
        Console.WriteLine("Please select a key pair type (type 1 or 2):");
        Console.WriteLine("1) Encryption");
        Console.WriteLine("2) Signing");
        string userInput = Console.ReadLine();
        Console.WriteLine();
        _ = int.TryParse(userInput, out int keyPairType);
        return keyPairType;
    }

    public static void RecoverPublicKey(string privateKeyPath, char[] password)
    {
        bool validUserInput = FilePathValidation.RecoverPublicKey(privateKeyPath);
        if (!validUserInput) {
            return;
        }
        try
        {
            Span<byte> privateKey = AsymmetricKeyValidation.GetPrivateKeyFromFile(privateKeyPath);
            privateKey = AsymmetricKeyValidation.DecryptPrivateKey(privateKeyPath, password, privateKey);
            Span<byte> publicKey = privateKey.Length switch
            {
                X25519.PrivateKeySize => AsymmetricKeys.GetCurve25519PublicKey(privateKey),
                _ => AsymmetricKeys.GetEd25519PublicKey(privateKey)
            };
            string publicKeyString = Encodings.ToBase64(publicKey);
            string publicKeyFilePath = AsymmetricKeys.ExportPublicKey(privateKeyPath, publicKeyString);
            DisplayMessage.PublicKey(publicKeyString, publicKeyFilePath);
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

    public static void Sign(string privateKeyPath, char[] password, string comment, bool prehash, string[] signatureFilePaths, string[] filePaths)
    {
        bool validUserInput = SigningValidation.Sign(privateKeyPath, comment, signatureFilePaths, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> privateKey = AsymmetricKeyValidation.SigningPrivateKeyFile(privateKeyPath, password);
        FileSigning.SignEachFile(filePaths, signatureFilePaths, comment, prehash, privateKey);
    }

    public static void Verify(string[] publicKeys, string[] signatureFilePaths, string[] filePaths)
    {
        if (publicKeys == null || publicKeys[0].EndsWith(Constants.PublicKeyExtension)) {
            VerifyWithPublicKeyFile(publicKeys, signatureFilePaths, filePaths);
            return;
        }
        VerifyWithPublicKeyString(publicKeys, signatureFilePaths, filePaths);
    }

    private static void VerifyWithPublicKeyFile(string[] publicKeyPaths, string[] signatureFilePaths, string[] filePaths)
    {
        bool validUserInput = SigningValidation.VerifyWithPublicKeyFile(publicKeyPaths, signatureFilePaths, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> publicKey = AsymmetricKeyValidation.SigningPublicKeyFile(publicKeyPaths[0]);
        FileSigning.VerifyEachFile(signatureFilePaths, filePaths, publicKey);
    }
    
    private static void VerifyWithPublicKeyString(string[] encodedPublicKeys, string[] signatureFilePaths, string[] filePaths)
    {
        bool validUserInput = SigningValidation.VerifyWithPublicKeyString(encodedPublicKeys, signatureFilePaths, filePaths);
        if (!validUserInput) {
            return;
        }
        Span<byte> publicKey = AsymmetricKeyValidation.SigningPublicKeyString(encodedPublicKeys[0]);
        FileSigning.VerifyEachFile(signatureFilePaths, filePaths, publicKey);
    }

    public static void CheckForUpdates()
    {
        try
        {
            bool updateAvailable = Updates.CheckForUpdates(out string latestVersion);
            if (!updateAvailable) {
                DisplayMessage.WriteLine("Kryptor is up-to-date.", ConsoleColor.Green);
                return;
            }
            Console.WriteLine($"An update is available for Kryptor. The latest version is v{latestVersion}.");
            Console.WriteLine();
            DisplayMessage.WriteLine($"IMPORTANT: Please check the latest changelog at <https://www.kryptor.co.uk/changelog#v{latestVersion}> to see if there are any breaking changes BEFORE updating.", ConsoleColor.Blue);
            Console.WriteLine();
            Console.WriteLine("Would you like Kryptor to automatically install this update now? (type y or n)");
            string userInput = Console.ReadLine()?.ToLower();
            if (!string.IsNullOrEmpty(userInput)) {
                Console.WriteLine();
            }
            switch (userInput)
            {
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

    public static void DisplayAbout()
    {
        Console.WriteLine($"Kryptor v{Program.GetVersion()}");
        Console.WriteLine("Copyright (C) 2020-2022 Samuel Lucas");
        Console.WriteLine("License GPLv3+: GNU GPL version 3 or later <https://www.gnu.org/licenses/gpl-3.0.html>.");
        Console.WriteLine("This is free software: you are free to change and redistribute it.");
        Console.WriteLine("There is NO WARRANTY, to the extent permitted by law.");
    }
}