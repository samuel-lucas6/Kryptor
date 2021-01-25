using System;
using System.Security.Cryptography;
using Sodium;

/*
    Kryptor: Free and open source file encryption.
    Copyright(C) 2020 Samuel Lucas

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
            byte[] magicBytes = FileHeaders.GetMagicBytes();
            byte[] fileFormatVersion = FileHeaders.GetFileFormatVersion();
            long chunkCount = Utilities.RoundUp(fileLength, Constants.FileChunkSize);
            byte[] ciphertextLength = BitConverter.GetBytes(chunkCount * Constants.TotalChunkLength);
            return Utilities.ConcatArrays(magicBytes, fileFormatVersion, ciphertextLength);
        }

        public static byte[] Encrypt(byte[] header, byte[] nonce, byte[] keyEncryptionKey, byte[] additionalData)
        {
            return SecretAeadXChaCha20Poly1305.Encrypt(header, nonce, keyEncryptionKey, additionalData);
        }

        public static byte[] GetAdditionalData(string inputFilePath)
        {
            byte[] magicBytes = FileHeaders.ReadMagicBytes(inputFilePath);
            byte[] fileFormatVersion = FileHeaders.ReadFileFormatVersion(inputFilePath);
            bool validFileFormat = Sodium.Utilities.Compare(fileFormatVersion, FileHeaders.GetFileFormatVersion());
            if (!validFileFormat) { throw new ArgumentOutOfRangeException(inputFilePath, "Incorrect file format for this version of Kryptor."); }
            long fileLength = FileHandling.GetFileLength(inputFilePath);
            int headersLength = FileHeaders.GetHeadersLength();
            byte[] ciphertextLength = BitConverter.GetBytes(fileLength - headersLength);
            return Utilities.ConcatArrays(magicBytes, fileFormatVersion, ciphertextLength);
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
