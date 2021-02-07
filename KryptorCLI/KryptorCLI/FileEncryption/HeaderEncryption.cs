using System;
using System.Security.Cryptography;
using Sodium;

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
            long chunkCount = (long)Math.Ceiling((double)fileLength / (double) Constants.FileChunkSize);
            byte[] ciphertextLength = BitConverter.GetBytes(chunkCount * Constants.TotalChunkLength);
            return Arrays.Concat(Constants.KryptorMagicBytes, Constants.EncryptionVersion, ciphertextLength);
        }

        public static byte[] Encrypt(byte[] header, byte[] nonce, byte[] keyEncryptionKey, byte[] additionalData)
        {
            return SecretAeadXChaCha20Poly1305.Encrypt(header, nonce, keyEncryptionKey, additionalData);
        }

        public static byte[] GetAdditionalData(string inputFilePath)
        {
            byte[] magicBytes = FileHeaders.ReadMagicBytes(inputFilePath);
            byte[] formatVersion = FileHeaders.ReadFileFormatVersion(inputFilePath);
            FileHeaders.ValidateFormatVersion(inputFilePath, formatVersion, Constants.EncryptionVersion);
            long fileLength = FileHandling.GetFileLength(inputFilePath);
            int headersLength = FileHeaders.GetHeadersLength();
            byte[] ciphertextLength = BitConverter.GetBytes(fileLength - headersLength);
            return Arrays.Concat(magicBytes, formatVersion, ciphertextLength);
        }

        public static byte[] Decrypt(byte[] encryptedHeader, byte[] nonce, byte[] keyEncryptionKey, byte[] additionalData)
        {
            try
            {
                return SecretAeadXChaCha20Poly1305.Decrypt(encryptedHeader, nonce, keyEncryptionKey, additionalData);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }
    }
}
