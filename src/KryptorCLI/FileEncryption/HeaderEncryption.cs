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
using System.IO;
using System.Security.Cryptography;
using ChaCha20BLAKE2;

namespace KryptorCLI;

public static class HeaderEncryption
{
    public static byte[] ComputeAdditionalData(long fileLength, byte[] ephemeralPublicKey)
    {
        long chunkCount = (long)Math.Ceiling((double)fileLength / Constants.FileChunkSize);
        byte[] ciphertextLength = BitConversion.GetBytes(chunkCount * Constants.CiphertextChunkLength);
        return Arrays.Concat(ciphertextLength, Constants.KryptorMagicBytes, Constants.EncryptionVersion, ephemeralPublicKey);
    }

    public static byte[] Encrypt(byte[] fileHeader, byte[] nonce, byte[] keyEncryptionKey, byte[] additionalData)
    {
        return XChaCha20BLAKE2b.Encrypt(fileHeader, nonce, keyEncryptionKey, additionalData);
    }

    public static byte[] GetAdditionalData(FileStream inputFile, byte[] ephemeralPublicKey)
    {
        byte[] ciphertextLength = BitConversion.GetBytes(inputFile.Length - Constants.FileHeadersLength);
        byte[] magicBytes = FileHeaders.ReadMagicBytes(inputFile);
        byte[] formatVersion = FileHeaders.ReadFileFormatVersion(inputFile);
        FileHeaders.ValidateFormatVersion(formatVersion, Constants.EncryptionVersion);
        return Arrays.Concat(ciphertextLength, magicBytes, formatVersion, ephemeralPublicKey);
    }

    public static byte[] Decrypt(byte[] encryptedFileHeader, byte[] nonce, byte[] keyEncryptionKey, byte[] additionalData)
    {
        try
        {
            return XChaCha20BLAKE2b.Decrypt(encryptedFileHeader, nonce, keyEncryptionKey, additionalData);
        }
        catch (CryptographicException ex)
        {
            throw new ArgumentException("Incorrect password/key, or this file has been tampered with.", ex);
        }
    }
}