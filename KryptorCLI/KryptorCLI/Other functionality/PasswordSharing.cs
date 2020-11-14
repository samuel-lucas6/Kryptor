using Sodium;
using System;
using System.Security.Cryptography;
using System.Text;

/*  
    Kryptor: Free and open source file encryption software.
    Copyright(C) 2020 Samuel Lucas

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace KryptorCLI
{
    public static class PasswordSharing
    {
        public static (string, string) GenerateKeyPair()
        {
            using (var keyPair = PublicKeyBox.GenerateKeyPair())
            {
                string publicKey = Convert.ToBase64String(keyPair.PublicKey);
                string privateKey = Convert.ToBase64String(keyPair.PrivateKey);
                return (publicKey, privateKey);
            }
        }

        public static char[] ConvertUserInput(bool encryption, char[] base64Key, char[] password)
        {
            try
            {
                NullChecks.CharArray(base64Key);
                NullChecks.CharArray(password);
                char[] message;
                byte[] key = Convert.FromBase64CharArray(base64Key, 0, base64Key.Length);
                if (encryption == true)
                {
                    byte[] plaintext = Encoding.UTF8.GetBytes(password);
                    message = EncryptPassword(plaintext, key);
                    Utilities.ZeroArray(plaintext);
                }
                else
                {
                    byte[] ciphertext = Convert.FromBase64CharArray(password, 0, password.Length);
                    message = DecryptPassword(ciphertext, key);
                    Utilities.ZeroArray(ciphertext);
                }
                Utilities.ZeroArray(key);
                return message;
            }
            catch (Exception ex) when (ex is FormatException || ex is EncoderFallbackException)
            {
                DisplayMessage.Error(ex.GetType().Name, "Invalid key or password format.");
                return Array.Empty<char>();
            }
        }

        private static char[] EncryptPassword(byte[] plaintext, byte[] publicKey)
        {
            try
            {
                byte[] ciphertext = SealedPublicKeyBox.Create(plaintext, publicKey);
                return Convert.ToBase64String(ciphertext).ToCharArray();
            }
            catch (Exception ex) when (ExceptionFilters.PasswordSharingExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                GetErrorMessage(ex);
                return Array.Empty<char>();
            }
        }

        private static char[] DecryptPassword(byte[] ciphertext, byte[] privateKey)
        {
            try
            {
                using (var keyPair = PublicKeyBox.GenerateKeyPair(privateKey))
                {
                    byte[] plaintext = SealedPublicKeyBox.Open(ciphertext, keyPair);
                    return Encoding.UTF8.GetChars(plaintext);
                }
            }
            catch (Exception ex) when (ExceptionFilters.PasswordSharingExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                GetErrorMessage(ex);
                return Array.Empty<char>();
            }
        }

        private static void GetErrorMessage(Exception ex)
        {
            string errorMessage;
            if (ex is CryptographicException)
            {
                errorMessage = $"Incorrect key/password.";
            }
            else if (ex is OverflowException)
            {
                errorMessage = $"The password must be in Base64 format.";
            }
            else if (ex is DecoderFallbackException)
            {
                errorMessage = "Unable to convert decrypted message to a char array.";
            }
            else
            {
                errorMessage = $"Invalid key length.";
            }
            DisplayMessage.Error(ex.GetType().Name, errorMessage);
        }
    }
}
