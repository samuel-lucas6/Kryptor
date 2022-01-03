/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2022 Samuel Lucas

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

using System;

namespace KryptorCLI;

public static class BitConversion
{
    public static byte[] GetBytes(short value) => ToLittleEndian(BitConverter.GetBytes(value));

    public static byte[] GetBytes(int value) => ToLittleEndian(BitConverter.GetBytes(value));

    public static byte[] GetBytes(long value) => ToLittleEndian(BitConverter.GetBytes(value));

    public static int ToInt32(byte[] value) => BitConverter.ToInt32(ToLittleEndian(value));

    private static byte[] ToLittleEndian(byte[] value)
    {
        if (!BitConverter.IsLittleEndian) { Array.Reverse(value); }
        return value;
    }
}