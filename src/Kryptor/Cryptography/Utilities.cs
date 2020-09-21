using System;

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

namespace Kryptor
{
    public static class Utilities
    {
        public static void ZeroArray(byte[] byteArray)
        {
            if (byteArray != null)
            {
                Array.Clear(byteArray, 0, byteArray.Length);
            }
        }

        public static void ZeroArray(char[] charArray)
        {
            if (charArray != null)
            {
                Array.Clear(charArray, 0, charArray.Length);
            }
        }

        public static byte[] XorBytes(byte[] keystream, byte[] fileBytes)
        {
            NullChecks.ByteArray(keystream);
            NullChecks.ByteArray(fileBytes);
            for (int i = 0; i < fileBytes.Length; i++)
            {
                fileBytes[i] = (byte)(fileBytes[i] ^ keystream[i]);
            }
            return fileBytes;
        }
    }
}
