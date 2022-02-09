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

using System.IO;
using System.Security.Cryptography;
using Sodium;

namespace KryptorCLI;

public static class Keyfiles
{
    public static void GenerateKeyfile(string keyfilePath)
    {
        var keyfileBytes = SodiumCore.GetRandomBytes(Constants.KeyfileLength);
        File.WriteAllBytes(keyfilePath, keyfileBytes);
        File.SetAttributes(keyfilePath, FileAttributes.ReadOnly);
        CryptographicOperations.ZeroMemory(keyfileBytes);
    }

    public static byte[] ReadKeyfile(string keyfilePath)
    {
        using var keyfile = new FileStream(keyfilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
        using var blake2b = new GenericHash.GenericHashAlgorithm(key: (byte[])null, Constants.HashLength);
        return blake2b.ComputeHash(keyfile);
    }
}