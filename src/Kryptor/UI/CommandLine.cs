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
using System.Net;
using System.Security.Cryptography;

namespace Kryptor;

public static class CommandLine
{
    public static void Encrypt(bool usePassword, char[] password, string keyfile, bool usePrivateKey, string privateKeyPath, string publicKey, string[] filePaths)
    {
        if (!usePrivateKey && string.IsNullOrEmpty(publicKey) && usePassword)
        {
            FileEncryptionWithPassword(password, keyfile, filePaths);
        }
        else if (!usePrivateKey && string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(keyfile))
        {
            FileEncryptionWithKeyfile(keyfile, filePaths);
        }
        else if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKeyPath) && string.IsNullOrEmpty(keyfile))
        {
            if (publicKey.EndsWith(Constants.PublicKeyExtension))
            {
                FileEncryptionWithPublicKeyFile(privateKeyPath, password, publicKey, filePaths);
                return;
            }
            FileEncryptionWithPublicKey(privateKeyPath, password, publicKey.ToCharArray(), filePaths);
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && string.IsNullOrEmpty(keyfile))
        {
            FileEncryptionWithPrivateKey(privateKeyPath, password, filePaths);
        }
        else
        {
            DisplayMessage.Error(ErrorMessages.PasswordBasedEncryption);
        }
    }

    private static void FileEncryptionWithPassword(char[] password, string keyfilePath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPassword(usePassword: true, keyfilePath, filePaths);
        if (!validUserInput) { return; }
        password = Password.GetNewPassword(password);
        var passwordBytes = Password.Prehash(password, FilePathValidation.KeyfilePath(keyfilePath));
        FileEncryption.EncryptEachFileWithPassword(filePaths, passwordBytes);
    }
    
    private static void FileEncryptionWithKeyfile(string keyfilePath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPassword(usePassword: false, keyfilePath, filePaths);
        if (!validUserInput) { return; }
        var keyfileBytes = Keyfiles.ReadKeyfile(FilePathValidation.KeyfilePath(keyfilePath));
        FileEncryption.EncryptEachFileWithKeyfile(filePaths, keyfileBytes);
    }

    private static void FileEncryptionWithPublicKeyFile(string senderPrivateKeyPath, char[] password, string recipientPublicKeyPath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPublicKey(senderPrivateKeyPath, recipientPublicKeyPath, filePaths);
        if (!validUserInput) { return; }
        byte[] senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath);
        byte[] recipientPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyFile(recipientPublicKeyPath);
        FileEncryption.EncryptEachFileWithPublicKey(senderPrivateKey, password, recipientPublicKey, filePaths);
    }

    private static void FileEncryptionWithPublicKey(string senderPrivateKeyPath, char[] password, char[] recipientPublicKeyString, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPublicKey(senderPrivateKeyPath, recipientPublicKeyString, filePaths);
        if (!validUserInput) { return; }
        byte[] senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath);
        byte[] recipientPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyString(recipientPublicKeyString);
        FileEncryption.EncryptEachFileWithPublicKey(senderPrivateKey, password, recipientPublicKey, filePaths);
    }

    private static void FileEncryptionWithPrivateKey(string privateKeyPath, char[] password, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPrivateKey(privateKeyPath, filePaths);
        if (!validUserInput) { return; }
        byte[] privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath);
        FileEncryption.EncryptEachFileWithPrivateKey(privateKey, password, filePaths);
    }

    public static void Decrypt(bool usePassword, char[] password, string keyfile, bool usePrivateKey, string privateKeyPath, string publicKey, string[] filePaths)
    {
        if (!usePrivateKey && string.IsNullOrEmpty(publicKey) && usePassword)
        {
            FileDecryptionWithPassword(password, keyfile, filePaths);
        }
        else if (!usePrivateKey && string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(keyfile))
        {
            FileDecryptionWithKeyfile(keyfile, filePaths);
        }
        else if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKeyPath) && string.IsNullOrEmpty(keyfile))
        {
            if (publicKey.EndsWith(Constants.PublicKeyExtension))
            {
                FileDecryptionWithPublicKeyFile(privateKeyPath, password, publicKey, filePaths);
                return;
            }
            FileDecryptionWithPublicKey(privateKeyPath, password, publicKey.ToCharArray(), filePaths);
        }
        else if (!string.IsNullOrEmpty(privateKeyPath) && string.IsNullOrEmpty(keyfile))
        {
            FileDecryptionWithPrivateKey(privateKeyPath, password, filePaths);
        }
        else
        {
            DisplayMessage.Error(ErrorMessages.PasswordBasedEncryption);
        }
    }

    private static void FileDecryptionWithPassword(char[] password, string keyfilePath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPassword(usePassword: true, keyfilePath, filePaths);
        if (!validUserInput) { return; }
        if (password.Length == 0) { password = PasswordPrompt.EnterYourPassword(); }
        var passwordBytes = Password.Prehash(password, keyfilePath);
        FileDecryption.DecryptEachFileWithPassword(filePaths, passwordBytes);
    }
    
    private static void FileDecryptionWithKeyfile(string keyfilePath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPassword(usePassword: false, keyfilePath, filePaths);
        if (!validUserInput) { return; }
        var keyfileBytes = Keyfiles.ReadKeyfile(keyfilePath);
        FileDecryption.DecryptEachFileWithKeyfile(filePaths, keyfileBytes);
    }

    private static void FileDecryptionWithPublicKeyFile(string recipientPrivateKeyPath, char[] password, string senderPublicKeyPath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPublicKey(recipientPrivateKeyPath, senderPublicKeyPath, filePaths);
        if (!validUserInput) { return; }
        byte[] recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath);
        byte[] senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyFile(senderPublicKeyPath);
        FileDecryption.DecryptEachFileWithPublicKey(recipientPrivateKey, password, senderPublicKey, filePaths);
    }

    private static void FileDecryptionWithPublicKey(string recipientPrivateKeyPath, char[] password, char[] senderPublicKeyString, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPublicKey(recipientPrivateKeyPath, senderPublicKeyString, filePaths);
        if (!validUserInput) { return; }
        byte[] recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath);
        byte[] senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyString(senderPublicKeyString);
        FileDecryption.DecryptEachFileWithPublicKey(recipientPrivateKey, password, senderPublicKey, filePaths);
    }

    private static void FileDecryptionWithPrivateKey(string privateKeyPath, char[] password, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPrivateKey(privateKeyPath, filePaths);
        if (!validUserInput) { return; }
        byte[] privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath);
        FileDecryption.DecryptEachFileWithPrivateKey(privateKey, password, filePaths);
    }

    public static void GenerateNewKeyPair(char[] password, string exportDirectoryPath)
    {
        try
        {
            int keyPairType = GetKeyPairType();
            bool validUserInput = FilePathValidation.GenerateKeyPair(exportDirectoryPath, keyPairType);
            if (!validUserInput) { return; }
            string publicKey, privateKey, publicKeyFilePath, privateKeyFilePath;
            if (keyPairType == 1)
            {
                (publicKey, privateKey) = AsymmetricKeys.GenerateEncryptionKeyPair(password);
                (publicKeyFilePath, privateKeyFilePath) = AsymmetricKeys.ExportKeyPair(exportDirectoryPath, Constants.DefaultEncryptionKeyFileName, publicKey, privateKey);
            }
            else
            {
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

    private static int GetKeyPairType()
    {
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
        if (!validUserInput) { return; }
        byte[] privateKey = AsymmetricKeyValidation.GetPrivateKeyFromFile(privateKeyPath);
        privateKey = PrivateKey.Decrypt(privateKey, password);
        if (privateKey == null) { return; }
        byte[] publicKey = privateKey.Length switch
        {
            Constants.EncryptionKeyLength => AsymmetricKeys.GetCurve25519PublicKey(privateKey),
            _ => AsymmetricKeys.GetEd25519PublicKey(privateKey)
        };
        string publicKeyString = Convert.ToBase64String(publicKey);
        string publicKeyFilePath = AsymmetricKeys.ExportPublicKey(privateKeyPath, publicKeyString);
        DisplayMessage.PublicKey(publicKeyString, publicKeyFilePath);
    }

    public static void Sign(string privateKeyPath, char[] password, string comment, bool prehash, string[] signatureFilePaths, string[] filePaths)
    {
        bool validUserInput = SigningValidation.Sign(privateKeyPath, comment, signatureFilePaths, filePaths);
        if (!validUserInput) { return; }
        byte[] privateKey = AsymmetricKeyValidation.SigningPrivateKeyFile(privateKeyPath);
        privateKey = PrivateKey.Decrypt(privateKey, password);
        FileSigning.SignEachFile(privateKey, comment, prehash, signatureFilePaths, filePaths);
    }

    public static void Verify(string publicKey, string[] signatureFilePaths, string[] filePaths)
    {
        if (string.IsNullOrEmpty(publicKey) || publicKey.EndsWith(Constants.PublicKeyExtension))
        {
            VerifySignatures(publicKey, signatureFilePaths, filePaths);
            return;
        }
        VerifySignatures(publicKey.ToCharArray(), signatureFilePaths, filePaths);
    }

    private static void VerifySignatures(string publicKeyPath, string[] signatureFilePaths, string[] filePaths)
    {
        bool validUserInput = SigningValidation.Verify(publicKeyPath, signatureFilePaths, filePaths);
        if (!validUserInput) { return; }
        byte[] publicKey = AsymmetricKeyValidation.SigningPublicKeyFile(publicKeyPath);
        FileSigning.VerifyEachFile(signatureFilePaths, filePaths, publicKey);
    }
    
    private static void VerifySignatures(char[] encodedPublicKey, string[] signatureFilePaths, string[] filePaths)
    {
        bool validUserInput = SigningValidation.Verify(encodedPublicKey, signatureFilePaths, filePaths);
        if (!validUserInput) { return; }
        byte[] publicKey = AsymmetricKeyValidation.SigningPublicKeyString(encodedPublicKey);
        FileSigning.VerifyEachFile(signatureFilePaths, filePaths, publicKey);
    }

    public static void CheckForUpdates()
    {
        try
        {
            bool updateAvailable = Updates.CheckForUpdates(out string latestVersion);
            if (!updateAvailable)
            {
                DisplayMessage.WriteLine("Kryptor is up-to-date.", ConsoleColor.Green);
                return;
            }
            Console.WriteLine($"An update is available for Kryptor. The latest version is v{latestVersion}.");
            Console.WriteLine();
            DisplayMessage.WriteLine($"IMPORTANT: Please check the latest changelog at <https://www.kryptor.co.uk/changelog#v{latestVersion}> to see if there are any breaking changes BEFORE updating.", ConsoleColor.Blue);
            Console.WriteLine();
            Console.WriteLine("Would you like Kryptor to automatically install this update now? (type y or n)");
            string userInput = Console.ReadLine()?.ToLower();
            if (!string.IsNullOrEmpty(userInput)) { Console.WriteLine(); }
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
            if (ex is PlatformNotSupportedException or CryptographicException)
            {
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