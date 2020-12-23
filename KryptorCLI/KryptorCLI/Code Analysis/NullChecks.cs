using System;
using System.IO;

/*
    Kryptor: Free and open source file encryption software.
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
    public static class NullChecks
    { 
        public static void StringArray(string[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
        }

        public static void ByteArray(byte[] byteArray)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException(nameof(byteArray));
            }
        }

        public static void Strings(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
        }

        public static void CharArray(char[] charArray)
        {
            if (charArray == null)
            {
                throw new ArgumentNullException(nameof(charArray));
            }
        }

        public static void FileHeaders(FileStream ciphertext, byte[] salt)
        {
            if (ciphertext == null)
            {
                throw new ArgumentNullException(nameof(ciphertext));
            }
            if (salt == null)
            {
                throw new ArgumentNullException(nameof(salt));
            }
        }
    }
}
