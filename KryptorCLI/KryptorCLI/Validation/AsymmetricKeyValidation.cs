using System;
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
    public static class AsymmetricKeyValidation
    {
        public static byte[] EncryptionPublicKeyFile(string publicKeyPath)
        {
            try
            {
                byte[] publicKey = GetPublicKeyFromFile(publicKeyPath);
                if (publicKey == null) { return null; }
                byte[] keyAlgorithm = GetKeyAlgorithm(publicKey);
                ValidateEncryptionKey(keyAlgorithm);
                return RemoveKeyAlgorithm(publicKey);
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                DisplayMessage.Exception(ex.GetType().Name, "Please specify a valid encryption public key.");
                return null;
            }
        }

        public static byte[] SigningPublicKeyFile(string publicKeyPath)
        {
            try
            {
                byte[] publicKey = GetPublicKeyFromFile(publicKeyPath);
                if (publicKey == null) { return null; }
                byte[] keyAlgorithm = GetKeyAlgorithm(publicKey);
                ValidateSigningKey(keyAlgorithm);
                return RemoveKeyAlgorithm(publicKey);
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                DisplayMessage.Exception(ex.GetType().Name, "Please specify a valid signing public key.");
                return null;
            }
        }

        private static byte[] GetPublicKeyFromFile(string publicKeyPath)
        {
            try
            {
                string encodedPublicKey = File.ReadAllText(publicKeyPath);
                if (encodedPublicKey.Length != Constants.PublicKeyLength)
                {
                    DisplayMessage.Error(ValidationMessages.PublicKeyString);
                    return null;
                }
                return Convert.FromBase64String(encodedPublicKey);
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                DisplayMessage.Exception(ex.GetType().Name, "Unable to retrieve public key.");
                return null;
            }
        }

        public static byte[] EncryptionPublicKeyString(char[] encodedPublicKey)
        {
            try
            {
                byte[] publicKey = ConvertPublicKeyString(encodedPublicKey);
                if (publicKey == null) { return null; }
                byte[] keyAlgorithm = GetKeyAlgorithm(publicKey);
                ValidateEncryptionKey(keyAlgorithm);
                return RemoveKeyAlgorithm(publicKey);
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                DisplayMessage.Exception(ex.GetType().Name, "Please enter a valid encryption public key.");
                return null;
            }
        }

        public static byte[] SigningPublicKeyString(char[] encodedPublicKey)
        {
            try
            {
                byte[] publicKey = ConvertPublicKeyString(encodedPublicKey);
                if (publicKey == null) { return null; }
                byte[] keyAlgorithm = GetKeyAlgorithm(publicKey);
                ValidateSigningKey(keyAlgorithm);
                return RemoveKeyAlgorithm(publicKey);
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                DisplayMessage.Exception(ex.GetType().Name, "Please enter a valid signing public key.");
                return null;
            }
        }

        private static byte[] ConvertPublicKeyString(char[] encodedPublicKey)
        {
            return Convert.FromBase64CharArray(encodedPublicKey, offset: 0, encodedPublicKey.Length);
        }

        private static byte[] GetKeyAlgorithm(byte[] asymmetricKey)
        {
            byte[] keyAlgorithm = new byte[Constants.Curve25519KeyHeader.Length];
            Array.Copy(asymmetricKey, keyAlgorithm, keyAlgorithm.Length);
            return keyAlgorithm;
        }

        private static void ValidateEncryptionKey(byte[] keyAlgorithm)
        {
            bool validKey = Sodium.Utilities.Compare(keyAlgorithm, Constants.Curve25519KeyHeader);
            if (!validKey) { throw new ArgumentException("Please specify an asymmetric encryption key."); }
        }

        private static void ValidateSigningKey(byte[] keyAlgorithm)
        {
            bool validKey = Sodium.Utilities.Compare(keyAlgorithm, Constants.Curve25519KeyHeader);
            if (!validKey) { throw new ArgumentException("Please specify an asymmetric signing key."); }
        }

        private static byte[] RemoveKeyAlgorithm(byte[] asymmetricKey)
        {
            byte[] publicKey = new byte[asymmetricKey.Length - Constants.Curve25519KeyHeader.Length];
            Array.Copy(asymmetricKey, Constants.Curve25519KeyHeader.Length, publicKey, destinationIndex: 0, publicKey.Length);
            return publicKey;
        }

        public static byte[] EncryptionPrivateKeyFile(string privateKeyPath)
        {
            try
            {
                byte[] privateKey = GetPrivateKeyFromFile(privateKeyPath);
                if (privateKey == null) { return null; }
                byte[] keyAlgorithm = GetKeyAlgorithm(privateKey);
                ValidateEncryptionKey(keyAlgorithm);
                return RemoveKeyAlgorithm(privateKey);
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                DisplayMessage.Exception(ex.GetType().Name, "Please specify a valid encryption private key.");
                return null;
            }
        }

        public static byte[] SigningPrivateKeyFile(string privateKeyPath)
        {
            try
            {
                byte[] privateKey = GetPrivateKeyFromFile(privateKeyPath);
                if (privateKey == null) { return null; }
                byte[] keyAlgorithm = GetKeyAlgorithm(privateKey);
                ValidateSigningKey(keyAlgorithm);
                return RemoveKeyAlgorithm(privateKey);
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                DisplayMessage.Exception(ex.GetType().Name, "Please specify a valid signing private key.");
                return null;
            }
        }

        public static byte[] GetPrivateKeyFromFile(string privateKeyPath)
        {
            try
            {
                string encodedPrivateKey = File.ReadAllText(privateKeyPath);
                if (encodedPrivateKey.Length != Constants.SigningPrivateKeyLength && encodedPrivateKey.Length != Constants.EncryptionPrivateKeyLength)
                {
                    DisplayMessage.Error(ValidationMessages.PrivateKeyFile);
                    return null;
                }
                byte[] privateKey = Convert.FromBase64String(encodedPrivateKey);
                ValidateKeyVersion(privateKey);
                return privateKey;
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                if (ex is ArgumentOutOfRangeException)
                {
                    DisplayMessage.Exception(ex.GetType().Name, ex.Message);
                    return null;
                }
                DisplayMessage.Exception(ex.GetType().Name, "Unable to retrieve private key.");
                return null;
            }
        }

        private static void ValidateKeyVersion(byte[] privateKey)
        {
            byte[] keyVersion = GetKeyVersion(privateKey);
            bool validKeyVersion = Sodium.Utilities.Compare(keyVersion, Constants.PrivateKeyVersion);
            if (!validKeyVersion) { throw new ArgumentOutOfRangeException("Unsupported private key version."); }
        }

        private static byte[] GetKeyVersion(byte[] privateKey)
        {
            byte[] keyVersion = new byte[Constants.PrivateKeyVersion.Length];
            Array.Copy(privateKey, Constants.Curve25519KeyHeader.Length, keyVersion, destinationIndex: 0, keyVersion.Length);
            return keyVersion;
        }
    }
}
