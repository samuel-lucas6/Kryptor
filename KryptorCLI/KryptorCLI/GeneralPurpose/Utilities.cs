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
    public static class Utilities
    {
        public static void ZeroArray(byte[] byteArray)
        {
            if (byteArray != null)
            {
                Array.Clear(byteArray, index: 0, byteArray.Length);
            }
        }

        public static void ZeroArray(char[] charArray)
        {
            if (charArray.Length > 0)
            {
                Array.Clear(charArray, index: 0, charArray.Length);
            }
        }

        public static byte[] ConcatArrays(byte[] a, byte[] b)
        {
            const int index = 0;
            byte[] concat = new byte[a.Length + b.Length];
            Array.Copy(a, index, concat, index, a.Length);
            Array.Copy(b, index, concat, a.Length, b.Length);
            return concat;
        }

        public static byte[] ConcatArrays(byte[] a, byte[] b, byte[] c)
        {
            const int index = 0;
            byte[] concat = new byte[a.Length + b.Length + c.Length];
            Array.Copy(a, index, concat, index, a.Length);
            Array.Copy(b, index, concat, a.Length, b.Length);
            Array.Copy(c, index, concat, a.Length + b.Length, c.Length);
            return concat;
        }

        public static byte[] ConcatArrays(byte[] a, byte[] b, byte[] c, byte[] d)
        {
            const int index = 0;
            byte[] concat = new byte[a.Length + b.Length + c.Length + d.Length];
            Array.Copy(a, index, concat, index, a.Length);
            Array.Copy(b, index, concat, a.Length, b.Length);
            Array.Copy(c, index, concat, a.Length + b.Length, c.Length);
            Array.Copy(d, index, concat, a.Length + b.Length + c.Length, d.Length);
            return concat;
        }

        public static byte[] ConcatArrays(byte[] a, byte[] b, byte[] c, byte[] d, byte[] e)
        {
            const int index = 0;
            byte[] concat = new byte[a.Length + b.Length + c.Length + d.Length + e.Length];
            Array.Copy(a, index, concat, index, a.Length);
            Array.Copy(b, index, concat, a.Length, b.Length);
            Array.Copy(c, index, concat, a.Length + b.Length, c.Length);
            Array.Copy(d, index, concat, a.Length + b.Length + c.Length, d.Length);
            Array.Copy(e, index, concat, a.Length + b.Length + c.Length + d.Length, e.Length);
            return concat;
        }

        public static bool Compare(char[] a, char[] b)
        {
            // Constant time comparison
            byte[] aBytes = Encoding.UTF8.GetBytes(a);
            byte[] bBytes = Encoding.UTF8.GetBytes(b);
            return Sodium.Utilities.Compare(aBytes, bBytes);
        }

        public static long RoundUp(long numerator, int denominator)
        {
            // More efficient than Math.Ceiling() that requires casting to decimal
            return ((numerator - 1) / denominator) + 1;
        }
    }
}
