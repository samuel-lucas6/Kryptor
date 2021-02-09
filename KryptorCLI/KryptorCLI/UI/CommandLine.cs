using System;
using System.IO;

/*
    Kryptor: Simple, modern, secure file encryption.
    Copyright(C) 2020-2021 Samuel Lucas

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

namespace KryptorCLI
{
    public static class CommandLine
    {
        public static void Encrypt(bool usePassword, string keyfile, string privateKey, string publicKey, string[] filePaths)
        {
            if (usePassword || !string.IsNullOrEmpty(keyfile))
            {
                char[] password = Array.Empty<char>();
                if (usePassword)
                {
                    password = PasswordPrompt.EnterNewPassword();
                }
                FileEncryptionWithPassword(password, keyfile, filePaths);
            }
            else if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKey))
            {
                if (publicKey.EndsWith(Constants.PublicKeyExtension))
                {
                    FileEncryptionWithPublicKey(privateKey, publicKey, filePaths);
                    return;
                }
                // Use private key string
                FileEncryptionWithPublicKey(privateKey, publicKey.ToCharArray(), filePaths);
            }
            else if (!string.IsNullOrEmpty(privateKey))
            {
                FileEncryptionWithPrivateKey(privateKey, filePaths);
            }
            else
            {
                DisplayMessage.Error("Please either specify a (password and/or keyfile), (private key and public key), or private key.");
            }
        }

        private static void FileEncryptionWithPassword(char[] password, string keyfilePath, string[] filePaths)
        {
            bool validUserInput = FileEncryptionValidation.FileEncryptionWithPassword(password, keyfilePath, filePaths);
            if (!validUserInput) { return; }
            keyfilePath = FilePathValidation.KeyfilePath(keyfilePath);
            byte[] passwordBytes = Password.Hash(password, keyfilePath);
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
                char[] password = Array.Empty<char>();
                if (usePassword)
                {
                    password = PasswordPrompt.EnterYourPassword();
                }
                FileDecryptionWithPassword(password, keyfile, filePaths);
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
                DisplayMessage.Error("Please either specify a (password and/or keyfile), (private key and public key), or private key.");
            }
        }

        private static void FileDecryptionWithPassword(char[] password, string keyfilePath, string[] filePaths)
        {
            bool validUserInput = FileEncryptionValidation.FileEncryptionWithPassword(password, keyfilePath, filePaths);
            if (!validUserInput) { return; }
            byte[] passwordBytes = Password.Hash(password, keyfilePath);
            FileDecryption.DecryptEachFileWithPassword(filePaths, passwordBytes);
        }

        private static void FileDecryptionWithPublicKey(string recipientPrivateKeyPath, string senderPublicKeyPath, string[] filePaths)
        {
            bool validUserInput = FileEncryptionValidation.FileEncryptionWithPublicKey(recipientPrivateKeyPath, senderPublicKeyPath, filePaths);
            if (!validUserInput) { return; }
            byte[] senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath);
            if (senderPrivateKey == null) { return; }
            byte[] recipientPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyFile(senderPublicKeyPath);
            if (recipientPublicKey == null) { return; }
            FileDecryption.DecryptEachFileWithPrivateKey(filePaths, senderPrivateKey, recipientPublicKey);
        }

        private static void FileDecryptionWithPublicKey(string recipientPrivateKeyPath, char[] senderPublicKeyString, string[] filePaths)
        {
            bool validUserInput = FileEncryptionValidation.FileEncryptionWithPublicKey(recipientPrivateKeyPath, senderPublicKeyString, filePaths);
            if (!validUserInput) { return; }
            byte[] senderPrivateKey = AsymmetricKeyValidation.EncryptionPrivateKeyFile(recipientPrivateKeyPath);
            if (senderPrivateKey == null) { return; }
            byte[] recipientPublicKey = AsymmetricKeyValidation.EncryptionPublicKeyString(senderPublicKeyString);
            if (recipientPublicKey == null) { return; }
            FileDecryption.DecryptEachFileWithPrivateKey(filePaths, senderPrivateKey, recipientPublicKey);
        }

        private static void FileDecryptionWithPrivateKey(string privateKeyPath, string[] filePaths)
        {
            bool validUserInput = FileEncryptionValidation.FileEncryptionWithPrivateKey(privateKeyPath, filePaths);
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
                    (publicKeyPath, privateKeyPath) = AsymmetricKeys.ExportEncryptionKeyPair(exportDirectoryPath, publicKey, privateKey);
                }
                else
                {
                    (publicKey, privateKey) = AsymmetricKeys.GenerateSigningKeyPair();
                    (publicKeyPath, privateKeyPath) = AsymmetricKeys.ExportSigningKeyPair(exportDirectoryPath, publicKey, privateKey);
                }
                DisplayKeyPair(publicKey, publicKeyPath, privateKeyPath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
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
            Arrays.Zero(privateKey);
            Console.WriteLine($"Public key: {Convert.ToBase64String(publicKey)}");
        }

        public static void Sign(string privateKeyPath, string comment, bool preHash, string[] filePaths)
        {
            bool validUserInput = SigningValidation.Sign(privateKeyPath, comment, filePaths);
            if (!validUserInput) { return; }
            byte[] privateKey = AsymmetricKeyValidation.SigningPrivateKeyFile(privateKeyPath);
            if (privateKey == null) { return; }
            FileSigning.SignEachFile(filePaths, comment, preHash, privateKey);
        }

        public static void Verify(string publicKey, string[] filePaths)
        {
            if (string.IsNullOrEmpty(publicKey) || publicKey.EndsWith(Constants.PublicKeyExtension))
            {
                // Use public key file
                VerifySignature(publicKey, filePaths);
                return;
            }
            // Use public key string
            VerifySignature(publicKey.ToCharArray(), filePaths);
        }

        private static void VerifySignature(char[] encodedPublicKey, string[] filePaths)
        {
            bool validUserInput = SigningValidation.Verify(encodedPublicKey, filePaths);
            if (!validUserInput) { return; }
            string signatureFilePath = SigningValidation.GetSignatureFilePath(ref filePaths);
            bool validSignatureFile = SigningValidation.SignatureFile(signatureFilePath, filePaths);
            if (!validSignatureFile) { return; }
            byte[] publicKey = AsymmetricKeyValidation.SigningPublicKeyString(encodedPublicKey);
            if (publicKey == null) { return; }
            FileSigning.VerifyFile(signatureFilePath, filePaths[0], publicKey);
        }

        private static void VerifySignature(string publicKeyPath, string[] filePaths)
        {
            bool validUserInput = SigningValidation.Verify(publicKeyPath, filePaths);
            if (!validUserInput) { return; }
            string signatureFilePath = SigningValidation.GetSignatureFilePath(ref filePaths);
            bool validSignatureFile = SigningValidation.SignatureFile(signatureFilePath, filePaths);
            if (!validSignatureFile) { return; }
            byte[] publicKey = AsymmetricKeyValidation.SigningPublicKeyFile(publicKeyPath);
            if (publicKey == null) { return; }
            FileSigning.VerifyFile(signatureFilePath, filePaths[0], publicKey);
        }
        
        public static void CheckForUpdates()
        {
            try
            {
                bool updateAvailable = Updates.CheckForUpdates();
                if (updateAvailable)
                {
                    Console.WriteLine("An update is available for Kryptor. Visit <https://github.com/samuel-lucas6/Kryptor/releases> to update.");
                    return;
                }
                Console.WriteLine("Kryptor is up-to-date.");
            }
            catch (Exception ex) when (ExceptionFilters.CheckForUpdates(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Warning);
                DisplayMessage.Exception(ex.GetType().Name, "Unable to check for updates.");
            }
        }

        public static void DisplayAbout()
        {
            Console.WriteLine($"Kryptor {Program.GetVersion()} Beta");
            Console.WriteLine("Copyright(C) 2020-2021 Samuel Lucas");
            Console.WriteLine("License GPLv3+: GNU GPL version 3 or later <https://gnu.org/licenses/gpl.html>");
            Console.WriteLine("This is free software: you are free to change and redistribute it.");
            Console.WriteLine("There is NO WARRANTY, to the extent permitted by law.");
        }
    }
}
