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
using System.Net;

namespace KryptorCLI;

public static class CommandLine
{
    public static void Encrypt(bool usePassword, string keyfile, string privateKey, string publicKey, string[] filePaths)
    {
        if (usePassword || !string.IsNullOrEmpty(keyfile))
        {
            FileEncryptionWithPassword(usePassword, keyfile, filePaths);
        }
        else if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKey))
        {
            if (publicKey.EndsWith(Constants.PublicKeyExtension))
            {
                // Use public key file
                FileEncryptionWithPublicKey(privateKey, publicKey, filePaths);
                return;
            }
            // Use public key string
            FileEncryptionWithPublicKey(privateKey, publicKey.ToCharArray(), filePaths);
        }
        else if (!string.IsNullOrEmpty(privateKey))
        {
            FileEncryptionWithPrivateKey(privateKey, filePaths);
        }
        else
        {
            DisplayMessage.Error("Please specify a password and/or keyfile, private key, or private key and public key.");
        }
    }

    private static void FileEncryptionWithPassword(bool usePassword, string keyfilePath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPassword(usePassword, keyfilePath, filePaths);
        if (!validUserInput) { return; }
        char[] password = Array.Empty<char>();
        if (usePassword)
        {
            password = PasswordPrompt.EnterNewPassword();
        }
        keyfilePath = FilePathValidation.KeyfilePath(keyfilePath);
        byte[] passwordBytes = Password.Prehash(password, keyfilePath);
        if (passwordBytes == null) { return; }
        FileEncryption.EncryptEachFileWithPassword(filePaths, passwordBytes);
    }

    private static void FileEncryptionWithPublicKey(string senderPrivateKeyPath, string recipientPublicKeyPath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPublicKey(senderPrivateKeyPath, recipientPublicKeyPath, filePaths);
        if (!validUserInput) { return; }
        byte[] senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath);
        if (senderPrivateKey == null) { return; }
        byte[] recipientPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyFile(recipientPublicKeyPath);
        if (recipientPublicKey == null) { return; }
        FileEncryption.EncryptEachFileWithPublicKey(filePaths, senderPrivateKey, recipientPublicKey);
    }

    private static void FileEncryptionWithPublicKey(string senderPrivateKeyPath, char[] recipientPublicKeyString, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPublicKey(senderPrivateKeyPath, recipientPublicKeyString, filePaths);
        if (!validUserInput) { return; }
        byte[] senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(senderPrivateKeyPath);
        if (senderPrivateKey == null) { return; }
        byte[] recipientPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyString(recipientPublicKeyString);
        if (recipientPublicKey == null) { return; }
        FileEncryption.EncryptEachFileWithPublicKey(filePaths, senderPrivateKey, recipientPublicKey);
    }

    private static void FileEncryptionWithPrivateKey(string privateKeyPath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileEncryptionWithPrivateKey(privateKeyPath, filePaths);
        if (!validUserInput) { return; }
        byte[] privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath);
        if (privateKey == null) { return; }
        FileEncryption.EncryptEachFileWithPrivateKey(filePaths, privateKey);
    }

    public static void Decrypt(bool usePassword, string keyfile, string privateKey, string publicKey, string[] filePaths)
    {
        if (usePassword || !string.IsNullOrEmpty(keyfile))
        {
            FileDecryptionWithPassword(usePassword, keyfile, filePaths);
        }
        else if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKey))
        {
            if (publicKey.EndsWith(Constants.PublicKeyExtension))
            {
                // Use public key file
                FileDecryptionWithPublicKey(privateKey, publicKey, filePaths);
                return;
            }
            // Use public key string
            FileDecryptionWithPublicKey(privateKey, publicKey.ToCharArray(), filePaths);
        }
        else if (!string.IsNullOrEmpty(privateKey))
        {
            FileDecryptionWithPrivateKey(privateKey, filePaths);
        }
        else
        {
            DisplayMessage.Error("Please specify a password and/or keyfile, private key, or private key and public key.");
        }
    }

    private static void FileDecryptionWithPassword(bool usePassword, string keyfilePath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPassword(usePassword, keyfilePath, filePaths);
        if (!validUserInput) { return; }
        char[] password = Array.Empty<char>();
        if (usePassword)
        {
            password = PasswordPrompt.EnterYourPassword();
        }
        byte[] passwordBytes = Password.Prehash(password, keyfilePath);
        if (passwordBytes == null) { return; }
        FileDecryption.DecryptEachFileWithPassword(filePaths, passwordBytes);
    }

    private static void FileDecryptionWithPublicKey(string recipientPrivateKeyPath, string senderPublicKeyPath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPublicKey(recipientPrivateKeyPath, senderPublicKeyPath, filePaths);
        if (!validUserInput) { return; }
        byte[] recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath);
        if (recipientPrivateKey == null) { return; }
        byte[] senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyFile(senderPublicKeyPath);
        if (senderPublicKey == null) { return; }
        FileDecryption.DecryptEachFileWithPublicKey(filePaths, recipientPrivateKey, senderPublicKey);
    }

    private static void FileDecryptionWithPublicKey(string recipientPrivateKeyPath, char[] senderPublicKeyString, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPublicKey(recipientPrivateKeyPath, senderPublicKeyString, filePaths);
        if (!validUserInput) { return; }
        byte[] recipientPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath);
        if (recipientPrivateKey == null) { return; }
        byte[] senderPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyString(senderPublicKeyString);
        if (senderPublicKey == null) { return; }
        FileDecryption.DecryptEachFileWithPublicKey(filePaths, recipientPrivateKey, senderPublicKey);
    }

    private static void FileDecryptionWithPrivateKey(string privateKeyPath, string[] filePaths)
    {
        bool validUserInput = FileEncryptionValidation.FileDecryptionWithPrivateKey(privateKeyPath, filePaths);
        if (!validUserInput) { return; }
        byte[] privateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(privateKeyPath);
        if (privateKey == null) { return; }
        FileDecryption.DecryptEachFileWithPrivateKey(filePaths, privateKey);
    }

    public static void GenerateNewKeyPair(string exportDirectoryPath)
    {
        try
        {
            int keyPairType = GetKeyPairType();
            bool validUserInput = FilePathValidation.GenerateKeyPair(exportDirectoryPath, keyPairType);
            if (!validUserInput) { return; }
            string publicKey, privateKey, publicKeyPath, privateKeyPath;
            if (keyPairType == 1)
            {
                (publicKey, privateKey) = AsymmetricKeys.GenerateEncryptionKeyPair();
                (publicKeyPath, privateKeyPath) = AsymmetricKeys.ExportKeyPair(exportDirectoryPath, Constants.DefaultEncryptionKeyFileName, publicKey, privateKey);
            }
            else
            {
                (publicKey, privateKey) = AsymmetricKeys.GenerateSigningKeyPair();
                (publicKeyPath, privateKeyPath) = AsymmetricKeys.ExportKeyPair(exportDirectoryPath, Constants.DefaultSigningKeyFileName, publicKey, privateKey);
            }
            DisplayKeyPair(publicKey, publicKeyPath, privateKeyPath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(exportDirectoryPath, ex.GetType().Name, "Unable to export key pair.");
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

    private static void DisplayKeyPair(string publicKey, string publicKeyPath, string privateKeyPath)
    {
        Console.WriteLine();
        Console.WriteLine($"Public key: {publicKey}");
        Console.WriteLine($"Public key file: {publicKeyPath}");
        Console.WriteLine();
        Console.WriteLine($"Private key file: {privateKeyPath} - Keep this secret!");
    }

    public static void RecoverPublicKey(string privateKeyPath)
    {
        bool validUserInput = FilePathValidation.RecoverPublicKey(privateKeyPath);
        if (!validUserInput) { return; }
        byte[] privateKey = AsymmetricKeyValidation.GetPrivateKeyFromFile(privateKeyPath);
        if (privateKey == null) { return; }
        privateKey = PrivateKey.Decrypt(privateKey);
        if (privateKey == null) { return; }
        byte[] publicKey = privateKey.Length switch
        {
            Constants.EncryptionKeyLength => AsymmetricKeys.GetCurve25519PublicKey(privateKey),
            _ => AsymmetricKeys.GetEd25519PublicKey(privateKey),
        };
        Console.WriteLine();
        Console.WriteLine($"Public key: {Convert.ToBase64String(publicKey)}");
    }

    public static void Sign(string privateKeyPath, string comment, bool preHash, string signatureFilePath, string[] filePaths)
    {
        bool validUserInput = SigningValidation.Sign(privateKeyPath, comment, signatureFilePath, filePaths);
        if (!validUserInput) { return; }
        byte[] privateKey = AsymmetricKeyValidation.SigningPrivateKeyFile(privateKeyPath);
        if (privateKey != null) { FileSigning.SignEachFile(filePaths, signatureFilePath, comment, preHash, privateKey); }
    }

    public static void Verify(string publicKey, string signatureFilePath, string[] filePaths)
    {
        if (string.IsNullOrEmpty(publicKey) || publicKey.EndsWith(Constants.PublicKeyExtension))
        {
            VerifySignature(publicKey, signatureFilePath, filePaths);
            return;
        }
        VerifySignature(publicKey.ToCharArray(), signatureFilePath, filePaths);
    }

    private static void VerifySignature(char[] encodedPublicKey, string signatureFilePath, string[] filePaths)
    {
        bool validUserInput = SigningValidation.Verify(encodedPublicKey, signatureFilePath, filePaths);
        if (!validUserInput) { return; }
        signatureFilePath = FilePathValidation.GetSignatureFilePath(signatureFilePath, filePaths);
        bool validSignatureFile = SigningValidation.SignatureFile(signatureFilePath);
        if (!validSignatureFile) { return; }
        byte[] publicKey = AsymmetricKeyValidation.SigningPublicKeyString(encodedPublicKey);
        if (publicKey != null) { FileSigning.VerifyFile(signatureFilePath, filePaths[0], publicKey); }
    }

    private static void VerifySignature(string publicKeyPath, string signatureFilePath, string[] filePaths)
    {
        signatureFilePath = FilePathValidation.GetSignatureFilePath(signatureFilePath, filePaths);
        bool validUserInput = SigningValidation.Verify(publicKeyPath, signatureFilePath, filePaths);
        if (!validUserInput) { return; }
        bool validSignatureFile = SigningValidation.SignatureFile(signatureFilePath);
        if (!validSignatureFile) { return; }
        byte[] publicKey = AsymmetricKeyValidation.SigningPublicKeyFile(publicKeyPath);
        if (publicKey != null) { FileSigning.VerifyFile(signatureFilePath, filePaths[0], publicKey); }
    }
        
    public static void CheckForUpdates()
    {
        try
        {
            bool updateAvailable = Updates.CheckForUpdates(out string latestVersion);
            if (!updateAvailable)
            {
                Console.WriteLine("Kryptor is up-to-date.");
                return;
            }
            Console.WriteLine("An update is available for Kryptor. Would you like to update now? (type y or n)");
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
                    Console.WriteLine("Please type either y or n next time.");
                    return;
            }
        }
        catch (Exception ex) when (ex is WebException or PlatformNotSupportedException || ExceptionFilters.FileAccess(ex))
        {
            if (ex is PlatformNotSupportedException)
            {
                DisplayMessage.Exception(ex.GetType().Name, ex.Message);
                return;
            }
            DisplayMessage.Exception(ex.GetType().Name, "Unable to check for or download updates.");
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