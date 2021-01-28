using System;
using Sodium;
using System.IO;

/*
    Kryptor: Free and open source file encryption.
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
        public static (string publicKey, string privateKey) Generate()
        {
            char[] password = PasswordPrompt.EnterNewPassword();
            byte[] passwordBytes = Password.Hash(password);
            using var keyPair = PublicKeyAuth.GenerateKeyPair();
            byte[] encryptedPrivateKey = PrivateKey.Encrypt(passwordBytes, keyPair.PrivateKey);
            return ConvertKeys(keyPair.PublicKey, encryptedPrivateKey);
        }

        private static (string publicKey, string privateKey) ConvertKeys(byte[] publicKey, byte[] encryptedPrivateKey)
        {
            return (Convert.ToBase64String(publicKey), Convert.ToBase64String(encryptedPrivateKey));
        }

        public static (string publicKeyPath, string privateKeyPath) Export(string directoryPath, string keyPairName, string publicKey, string privateKey)
        {
            CreateKeysDirectory(directoryPath);
            string publicKeyPath = Path.Combine(directoryPath, keyPairName + Constants.PublicKeyExtension);
            CreateKeyFile(publicKeyPath, publicKey);
            string privateKeyPath = Path.Combine(directoryPath, keyPairName + Constants.PrivateKeyExtension);
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
            File.WriteAllText(filePath, asymmetricKey);
            File.SetAttributes(filePath, FileAttributes.ReadOnly);
        }

        public static byte[] ExtractPublicKey(byte[] privateKey)
        {
            return PublicKeyAuth.ExtractEd25519PublicKeyFromEd25519SecretKey(privateKey);
        }
    }
}
