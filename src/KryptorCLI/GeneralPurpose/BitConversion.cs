using System;

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
    public static class BitConversion
    {
        public static byte[] GetBytes(int value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            return ToLittleEndian(valueBytes);
        }

        public static byte[] GetBytes(short value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            return ToLittleEndian(valueBytes);
        }

        public static byte[] GetBytes(long value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            return ToLittleEndian(valueBytes);
        }

        public static int ToInt32(byte[] value)
        {
            value = ToLittleEndian(value);
            return BitConverter.ToInt32(value);
        }

        private static byte[] ToLittleEndian(byte[] value)
        {
            // Always use little endian
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(value);
            }
            return value;
        }
    }
}
