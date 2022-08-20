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
using System.IO;
using System.Text;
using Geralt;

namespace Kryptor;

public static class SymmetricKeyValidation
{
    public static byte[] GetEncryptionSymmetricKey(string symmetricKey)
    {
        try
        {
            if (string.IsNullOrEmpty(symmetricKey)) {
                return null;
            }
            if (ConstantTime.Equals(Encoding.UTF8.GetBytes(symmetricKey), Encoding.UTF8.GetBytes(" "))) {
                var key = GC.AllocateArray<byte>(ChaCha20.KeySize, pinned: true);
                SecureRandom.Fill(key);
                DisplayMessage.SymmetricKey(Encodings.ToBase64(Arrays.Concat(Constants.SymmetricKeyHeader, key)));
                return key;
            }
            if (ConstantTime.Equals(Encoding.UTF8.GetBytes(new[] { symmetricKey[^1] }), Constants.Base64Padding) ) {
                return KeyString(symmetricKey);
            }
            if (File.Exists(symmetricKey)) {
                return ReadKeyfile(symmetricKey);
            }
            if (Directory.Exists(symmetricKey)) {
                symmetricKey = Path.Combine(symmetricKey, SecureRandom.GetString(Constants.RandomFileNameLength));
            }
            if (!symmetricKey.EndsWith(Constants.KeyfileExtension)) {
                symmetricKey += Constants.KeyfileExtension;
            }
            if (File.Exists(symmetricKey)) {
                return ReadKeyfile(symmetricKey);
            }
            var keyfileBytes = GC.AllocateArray<byte>(Constants.KeyfileLength, pinned: true);
            SecureRandom.Fill(keyfileBytes);
            File.WriteAllBytes(symmetricKey, keyfileBytes);
            File.SetAttributes(symmetricKey, FileAttributes.ReadOnly);
            DisplayMessage.Keyfile(symmetricKey);
            return keyfileBytes;
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(symmetricKey, ex.GetType().Name, "Unable to randomly generate a keyfile.");
            return null;
        }
    }
    
    public static byte[] GetDecryptionSymmetricKey(string symmetricKey)
    {
        if (string.IsNullOrEmpty(symmetricKey)) {
            return null;
        }
        return ConstantTime.Equals(Encoding.UTF8.GetBytes(new[] { symmetricKey[^1] }), Constants.Base64Padding)  ? KeyString(symmetricKey) : ReadKeyfile(symmetricKey);
    }

    private static byte[] KeyString(string encodedSymmetricKey)
    {
        try
        {
            if (encodedSymmetricKey.Length != Constants.SymmetricKeyLength) {
                throw new ArgumentException(ErrorMessages.InvalidSymmetricKey);
            }
            byte[] symmetricKey = Encodings.FromBase64(encodedSymmetricKey);
            byte[] keyHeader = Arrays.Slice(symmetricKey, sourceIndex: 0, Constants.SymmetricKeyHeader.Length);
            bool validKey = ConstantTime.Equals(keyHeader, Constants.SymmetricKeyHeader);
            if (!validKey) {
                throw new NotSupportedException("This isn't a symmetric key.");
            }
            return symmetricKey[Constants.SymmetricKeyHeader.Length..];
        }
        catch (Exception ex)
        {
            DisplayMessage.KeyStringException(encodedSymmetricKey, ex.GetType().Name, ErrorMessages.InvalidSymmetricKey);
            return null;
        }
    }

    private static byte[] ReadKeyfile(string keyfilePath)
    {
        try
        {
            using var keyfile = new FileStream(keyfilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
            using var blake2b = new BLAKE2bHashAlgorithm(BLAKE2b.MaxHashSize);
            return blake2b.ComputeHash(keyfile);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(keyfilePath, ex.GetType().Name, "Unable to read the keyfile.");
            return null;
        }
    }
}