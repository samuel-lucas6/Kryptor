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
using System.Security.Cryptography;
using Geralt;

namespace Kryptor;

public static class SymmetricKeyValidation
{
    public static Span<byte> GetEncryptionSymmetricKey(string symmetricKey)
    {
        if (string.IsNullOrEmpty(symmetricKey)) {
            return Span<byte>.Empty;
        }
        Span<byte> symmetricKeyBytes = GC.AllocateArray<byte>(Encoding.UTF8.GetMaxByteCount(symmetricKey.Length), pinned: true);
        int bytesEncoded = Encoding.UTF8.GetBytes(symmetricKey, symmetricKeyBytes);
        if (ConstantTime.Equals(symmetricKeyBytes[..bytesEncoded], Encoding.UTF8.GetBytes(" "))) {
            return GenerateKeyString();
        }
        if (symmetricKey.EndsWith(Constants.Base64Padding)) {
            CryptographicOperations.ZeroMemory(symmetricKeyBytes);
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
        return File.Exists(symmetricKey) ? ReadKeyfile(symmetricKey) : GenerateKeyfile(symmetricKey);
    }
    
    public static Span<byte> GetDecryptionSymmetricKey(string symmetricKey)
    {
        if (string.IsNullOrEmpty(symmetricKey)) {
            return Span<byte>.Empty;
        }
        return symmetricKey.EndsWith(Constants.Base64Padding) ? KeyString(symmetricKey) : ReadKeyfile(symmetricKey);
    }

    private static Span<byte> GenerateKeyString()
    {
        Span<byte> key = GC.AllocateArray<byte>(ChaCha20.KeySize, pinned: true);
        SecureRandom.Fill(key);
        Span<byte> keyString = stackalloc byte[Constants.SymmetricKeyHeader.Length + key.Length];
        Spans.Concat(keyString, Constants.SymmetricKeyHeader, key);
        DisplayMessage.SymmetricKey(Encodings.ToBase64(keyString));
        CryptographicOperations.ZeroMemory(keyString);
        return key;
    }

    private static Span<byte> GenerateKeyfile(string filePath)
    {
        try
        {
            var keyfileBytes = GC.AllocateArray<byte>(Constants.KeyfileLength, pinned: true);
            SecureRandom.Fill(keyfileBytes);
            File.WriteAllBytes(filePath, keyfileBytes);
            CryptographicOperations.ZeroMemory(keyfileBytes);
            File.SetAttributes(filePath, FileAttributes.ReadOnly);
            DisplayMessage.Keyfile(filePath);
            return ReadKeyfile(filePath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(filePath, ex.GetType().Name, "Unable to randomly generate a keyfile.");
            throw new UserInputException(ex.Message, ex);
        }
    }

    private static Span<byte> KeyString(string encodedSymmetricKey)
    {
        try
        {
            if (encodedSymmetricKey.Length != Constants.SymmetricKeyLength) {
                throw new ArgumentException(ErrorMessages.InvalidSymmetricKey);
            }
            Span<byte> symmetricKey = Encodings.FromBase64(encodedSymmetricKey);
            Span<byte> keyHeader = symmetricKey[..Constants.SymmetricKeyHeader.Length];
            if (!ConstantTime.Equals(keyHeader, Constants.SymmetricKeyHeader)) {
                throw new NotSupportedException("This isn't a symmetric key.");
            }
            return symmetricKey[Constants.SymmetricKeyHeader.Length..];
        }
        catch (Exception ex) when (ExceptionFilters.StringKey(ex))
        {
            DisplayMessage.KeyStringException(encodedSymmetricKey, ex.GetType().Name, ErrorMessages.InvalidSymmetricKey);
            throw new UserInputException(ex.Message, ex);
        }
    }

    private static Span<byte> ReadKeyfile(string keyfilePath)
    {
        try
        {
            using var keyfile = new FileStream(keyfilePath, FileHandling.GetFileStreamReadOptions(keyfilePath));
            using var blake2b = new BLAKE2bHashAlgorithm(Constants.KeyfileLength);
            return blake2b.ComputeHash(keyfile);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(keyfilePath, ex.GetType().Name, "Unable to read the keyfile.");
            throw new UserInputException(ex.Message, ex);
        }
    }
}