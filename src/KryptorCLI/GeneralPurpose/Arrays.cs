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
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Sodium;

namespace KryptorCLI;

public static class Arrays
{
    public static T[] Concat<T>(params T[][] arrays)
    {
        int offset = 0;
        var result = new T[arrays.Sum(array => array.Length)];
        foreach (var array in arrays)
        {
            Array.Copy(array, sourceIndex: 0, result, offset, array.Length);
            offset += array.Length;
        }
        return result;
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
        var bBytes = Encoding.UTF8.GetBytes(b);
        var key = SodiumCore.GetRandomBytes(Constants.BLAKE2Length);
        aBytes = Blake2b.KeyedHash(aBytes, key);
        bBytes = Blake2b.KeyedHash(bBytes, key);
        CryptographicOperations.ZeroMemory(key);
        return Utilities.Compare(aBytes, bBytes);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ZeroMemory(char[] array)
    {
        if (array.Length <= 0) { return; }
        Array.Clear(array, index: 0, array.Length);
    }
}