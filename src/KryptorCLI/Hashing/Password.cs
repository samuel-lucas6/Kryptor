using System;
using System.Text;
using System.Security.Cryptography;

/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2021 Samuel Lucas

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
        public static byte[] Prehash(char[] password, string keyfilePath)
        {
            byte[] passwordBytes = Prehash(password);
            if (!string.IsNullOrEmpty(keyfilePath)) { passwordBytes = UseKeyfile(passwordBytes, keyfilePath); }
            return passwordBytes;
        }

        public static byte[] Prehash(char[] password)
        {
            if (password.Length == 0) { return null; }
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            Arrays.ZeroMemory(password);
            return Blake2b.Hash(passwordBytes);
        }

        private static byte[] UseKeyfile(byte[] passwordBytes, string keyfilePath)
        {
            try
            {
                byte[] keyfileBytes = Keyfiles.ReadKeyfile(keyfilePath);
                return passwordBytes == null ? keyfileBytes : PepperPassword(passwordBytes, keyfileBytes);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                DisplayMessage.Exception(ex.GetType().Name, "Unable to read keyfile. The keyfile has not been used.");
                return passwordBytes;
            }
        }

        private static byte[] PepperPassword(byte[] passwordBytes, byte[] keyfileBytes)
        {
            passwordBytes = Blake2b.KeyedHash(passwordBytes, keyfileBytes);
            CryptographicOperations.ZeroMemory(keyfileBytes);
            return passwordBytes;
        }
    }
}
