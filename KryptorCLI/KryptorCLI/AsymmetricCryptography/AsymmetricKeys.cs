using System;
using Sodium;
using System.IO;

/*
    Kryptor: A simple, modern, and secure encryption tool.
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
    public static class AsymmetricKeys
    {
        public static (string publicKey, string privateKey) GenerateEncryptionKeyPair()
        {
            char[] password = PasswordPrompt.EnterNewPassword();
            byte[] passwordBytes = Password.Hash(password);
            using var keyPair = PublicKeyBox.GenerateKeyPair();
            byte[] publicKey = Arrays.Concat(Constants.Curve25519KeyHeader, keyPair.PublicKey);
            byte[] encryptedPrivateKey = PrivateKey.Encrypt(passwordBytes, Constants.Curve25519KeyHeader, keyPair.PrivateKey);
            return ConvertKeys(publicKey, encryptedPrivateKey);
        }

        public static (string publicKey, string privateKey) GenerateSigningKeyPair()
        {
            char[] password = PasswordPrompt.EnterNewPassword();
            byte[] passwordBytes = Password.Hash(password);
            using var keyPair = PublicKeyAuth.GenerateKeyPair();
            byte[] publicKey = Arrays.Concat(Constants.Ed25519KeyHeader, keyPair.PublicKey);
            byte[] encryptedPrivateKey = PrivateKey.Encrypt(passwordBytes, Constants.Ed25519KeyHeader, keyPair.PrivateKey);
            return ConvertKeys(publicKey, encryptedPrivateKey);
        }

        private static (string publicKey, string privateKey) ConvertKeys(byte[] publicKey, byte[] encryptedPrivateKey)
        {
            return (Convert.ToBase64String(publicKey), Convert.ToBase64String(encryptedPrivateKey));
        }

        public static (string publicKeyPath, string privateKeyPath) ExportEncryptionKeyPair(string directoryPath, string publicKey, string privateKey)
        {
            CreateKeysDirectory(directoryPath);
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PublicKeyExtension);
            CreateKeyFile(publicKeyPath, publicKey);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultEncryptionKeyFileName + Constants.PrivateKeyExtension);
            CreateKeyFile(privateKeyPath, privateKey);
            return (publicKeyPath, privateKeyPath);
        }

        public static (string publicKeyPath, string privateKeyPath) ExportSigningKeyPair(string directoryPath, string publicKey, string privateKey)
        {
            CreateKeysDirectory(directoryPath);
            string publicKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PublicKeyExtension);
            CreateKeyFile(publicKeyPath, publicKey);
            string privateKeyPath = Path.Combine(directoryPath, Constants.DefaultSigningKeyFileName + Constants.PrivateKeyExtension);
            CreateKeyFile(privateKeyPath, privateKey);
            return (publicKeyPath, privateKeyPath);
        }

        private static void CreateKeysDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private static void CreateKeyFile(string filePath, string asymmetricKey)
        {
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
            File.WriteAllText(filePath, asymmetricKey);
            File.SetAttributes(filePath, FileAttributes.ReadOnly);
        }

        public static byte[] GetCurve25519PublicKey(byte[] privateKey)
        {
            byte[] publicKey = ScalarMult.Base(privateKey);
            return Arrays.Concat(Constants.Curve25519KeyHeader, publicKey);
        }

        public static byte[] GetEd25519PublicKey(byte[] privateKey)
        {
            byte[] publicKey = PublicKeyAuth.ExtractEd25519PublicKeyFromEd25519SecretKey(privateKey);
            return Arrays.Concat(Constants.Ed25519KeyHeader, publicKey);
        }
    }
}
