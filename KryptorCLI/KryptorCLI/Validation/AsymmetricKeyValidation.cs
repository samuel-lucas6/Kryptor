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
        public static byte[] GetPublicKeyFromFile(string publicKeyPath)
        {
            try
            {
                string encodedPublicKey = File.ReadAllText(publicKeyPath);
                if (encodedPublicKey.Length != Constants.PublicKeyLength)
                {
                    DisplayMessage.Error(ValidationMessages.PublicKeyString);
                    return null;
                }
                byte[] publicKey = Convert.FromBase64String(encodedPublicKey);
                ValidateKeyVersion(publicKey);
                return RemoveKeyVersion(publicKey);
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                if (ex is ArgumentOutOfRangeException)
                {
                    DisplayMessage.Exception(ex.GetType().Name, ex.Message);
                    return null;
                }
                DisplayMessage.Exception(ex.GetType().Name, "Unable to retrieve public key.");
                return null;
            }
        }

        public static byte[] GetPrivateKeyFromFile(string privateKeyPath)
        {
            try
            {
                string encodedPrivateKey = File.ReadAllText(privateKeyPath);
                if (encodedPrivateKey.Length != Constants.PrivateKeyLength)
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

        public static byte[] ConvertPublicKeyString(char[] encodedPublicKey)
        {
            try
            {
                byte[] publicKey = Convert.FromBase64CharArray(encodedPublicKey, offset: 0, encodedPublicKey.Length);
                ValidateKeyVersion(publicKey);
                return RemoveKeyVersion(publicKey);
            }
            catch (Exception ex) when (ExceptionFilters.AsymmetricKeyHandling(ex))
            {
                if (ex is ArgumentOutOfRangeException)
                {
                    DisplayMessage.Exception(ex.GetType().Name, ex.Message);
                    return null;
                }
                DisplayMessage.Exception(ex.GetType().Name, "Invalid public key format.");
                return null;
            }
        }

        private static void ValidateKeyVersion(byte[] asymmetricKey)
        {
            byte[] keyVersion = GetKeyVersion(asymmetricKey);
            bool validKeyVersion = Sodium.Utilities.Compare(keyVersion, Constants.KeyVersion);
            if (!validKeyVersion) { throw new ArgumentOutOfRangeException("Unsupported key version."); }
        }

        private static byte[] GetKeyVersion(byte[] asymmetricKey)
        {
            byte[] keyVersion = new byte[Constants.KeyVersion.Length];
            Array.Copy(asymmetricKey, keyVersion, keyVersion.Length);
            return keyVersion;
        }

        private static byte[] RemoveKeyVersion(byte[] asymmetricKey)
        {
            byte[] key = new byte[asymmetricKey.Length - Constants.KeyVersion.Length];
            Array.Copy(asymmetricKey, Constants.KeyVersion.Length, key, destinationIndex: 0, key.Length);
            return key;
        }
    }
}
