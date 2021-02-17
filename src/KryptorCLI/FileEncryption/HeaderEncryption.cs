using System;
using System.IO;
using System.Security.Cryptography;
using Sodium;
using ChaCha20BLAKE2;

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
    public static class HeaderEncryption
    {
        public static byte[] ComputeAdditionalData(long fileLength)
        {
            long chunkCount = (long)Math.Ceiling((double)fileLength / Constants.FileChunkSize);
            byte[] ciphertextLength = BitConversion.GetBytes(chunkCount * Constants.TotalChunkLength);
            return Arrays.Concat(Constants.KryptorMagicBytes, Constants.EncryptionVersion, ciphertextLength);
        }

        public static byte[] Encrypt(byte[] fileHeader, byte[] nonce, byte[] keyEncryptionKey, byte[] additionalData)
        {
            return XChaCha20BLAKE2b.Encrypt(fileHeader, nonce, keyEncryptionKey, additionalData, TagLength.Medium);
        }

        public static byte[] GetAdditionalData(FileStream inputFile)
        {
            byte[] magicBytes = FileHeaders.ReadMagicBytes(inputFile);
            byte[] formatVersion = FileHeaders.ReadFileFormatVersion(inputFile);
            FileHeaders.ValidateFormatVersion(formatVersion, Constants.EncryptionVersion);
            int headersLength = FileHeaders.GetHeadersLength();
            byte[] ciphertextLength = BitConversion.GetBytes(inputFile.Length - headersLength);
            return Arrays.Concat(magicBytes, formatVersion, ciphertextLength);
        }

        public static byte[] Decrypt(byte[] encryptedFileHeader, byte[] nonce, byte[] keyEncryptionKey, byte[] additionalData)
        {
            try
            {
                return XChaCha20BLAKE2b.Decrypt(encryptedFileHeader, nonce, keyEncryptionKey, additionalData, TagLength.Medium);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }
    }
}
