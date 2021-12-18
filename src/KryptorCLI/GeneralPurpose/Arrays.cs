/*
    Kryptor: A simple, modern, and secure encryption tool.
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
using System.Text;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Sodium;

namespace KryptorCLI;

public static class Arrays
{
    private const int ZeroIndex = 0;

    public static byte[] Concat(byte[] a, byte[] b)
    {
        var concat = new byte[a.Length + b.Length];
        Array.Copy(a, ZeroIndex, concat, ZeroIndex, a.Length);
        Array.Copy(b, ZeroIndex, concat, a.Length, b.Length);
        return concat;
    }

    public static byte[] Concat(byte[] a, byte[] b, byte[] c)
    {
        var concat = new byte[a.Length + b.Length + c.Length];
        Array.Copy(a, ZeroIndex, concat, ZeroIndex, a.Length);
        Array.Copy(b, ZeroIndex, concat, a.Length, b.Length);
        Array.Copy(c, ZeroIndex, concat, a.Length + b.Length, c.Length);
        return concat;
    }

    public static byte[] Concat(byte[] a, byte[] b, byte[] c, byte[] d)
    {
        var concat = new byte[a.Length + b.Length + c.Length + d.Length];
        Array.Copy(a, ZeroIndex, concat, ZeroIndex, a.Length);
        Array.Copy(b, ZeroIndex, concat, a.Length, b.Length);
        Array.Copy(c, ZeroIndex, concat, a.Length + b.Length, c.Length);
        Array.Copy(d, ZeroIndex, concat, a.Length + b.Length + c.Length, d.Length);
        return concat;
    }

    public static byte[] Concat(byte[] a, byte[] b, byte[] c, byte[] d, byte[] e)
    {
        var concat = new byte[a.Length + b.Length + c.Length + d.Length + e.Length];
        Array.Copy(a, ZeroIndex, concat, ZeroIndex, a.Length);
        Array.Copy(b, ZeroIndex, concat, a.Length, b.Length);
        Array.Copy(c, ZeroIndex, concat, a.Length + b.Length, c.Length);
        Array.Copy(d, ZeroIndex, concat, a.Length + b.Length + c.Length, d.Length);
        Array.Copy(e, ZeroIndex, concat, a.Length + b.Length + c.Length + d.Length, e.Length);
        return concat;
    }

    public static byte[] Copy(byte[] sourceArray, int sourceIndex, int length)
    {
        var destinationArray = new byte[length];
        Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex: 0, destinationArray.Length);
        return destinationArray;
    }

    public static bool Compare(char[] a, char[] b)
    {
        var aBytes = Encoding.UTF8.GetBytes(a);
        byte[] aHash = Blake2b.Hash(aBytes);
        CryptographicOperations.ZeroMemory(aBytes);
        var bBytes = Encoding.UTF8.GetBytes(b);
        byte[] bHash = Blake2b.Hash(bBytes);
        CryptographicOperations.ZeroMemory(bBytes);
        bool equal = Utilities.Compare(aHash, bHash);
        CryptographicOperations.ZeroMemory(aHash);
        CryptographicOperations.ZeroMemory(bHash);
        return equal;
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ZeroMemory(char[] array)
    {
        if (array.Length > 0)
        {
            Array.Clear(array, ZeroIndex, array.Length);
        }
    }
}