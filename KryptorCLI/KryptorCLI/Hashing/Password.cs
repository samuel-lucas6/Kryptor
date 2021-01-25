using System;
using System.Text;

/*
    Kryptor: Free and open source file encryption.
    Copyright(C) 2020 Samuel Lucas

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
    public static class Password
    {
        public static byte[] GetFileEncryptionPassword(char[] password, string keyfilePath)
        {
            byte[] passwordBytes = Hash(password);
            if (!string.IsNullOrEmpty(keyfilePath))
            {
                passwordBytes = UseKeyfile(passwordBytes, keyfilePath);
            }
            return passwordBytes;
        }

        public static byte[] Hash(char[] password)
        {
            byte[] passwordBytes = GetPasswordBytes(password);
            return GetPasswordHash(passwordBytes);
        }

        private static byte[] GetPasswordBytes(char[] password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            Utilities.ZeroArray(password);
            return passwordBytes;
        }

        private static byte[] GetPasswordHash(byte[] passwordBytes)
        {
            return Blake2.Hash(passwordBytes);
        }

        private static byte[] UseKeyfile(byte[] passwordBytes, string keyfilePath)
        {
            if (passwordBytes == null)
            {
                // If only a keyfile was selected
                return KeyfileAsPassword(keyfilePath);
            }
            return CombineKeyfileAndPassword(keyfilePath, passwordBytes);
        }

        public static byte[] CombineKeyfileAndPassword(string keyfilePath, byte[] passwordBytes)
        {
            try
            {
                byte[] keyfileBytes = Keyfiles.ReadKeyfile(keyfilePath);
                passwordBytes = Blake2.KeyedHash(passwordBytes, keyfileBytes);
                Utilities.ZeroArray(keyfileBytes);
                return passwordBytes;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.Exception(ex.GetType().Name, "Unable to read keyfile. The selected keyfile has not been used.");
                return passwordBytes;
            }
        }

        public static byte[] KeyfileAsPassword(string keyfilePath)
        {
            try
            {
                byte[] passwordBytes = Keyfiles.ReadKeyfile(keyfilePath);
                return GetPasswordHash(passwordBytes);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Error);
                DisplayMessage.Exception(ex.GetType().Name, "Unable to read keyfile. The selected keyfile has not been used.");
                return null;
            }
        }
    }
}
