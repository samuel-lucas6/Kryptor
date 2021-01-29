using System;
using System.IO;
using System.Text;

/*
    Kryptor: Free and open source file encryption.
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
    public static class FileHeaders
    {
        private static readonly byte[] _magicBytes = Encoding.UTF8.GetBytes(Constants.KryptorMagicBytes);
        private static readonly byte[] _formatVersion = BitConverter.GetBytes(Constants.EncryptionFormatVersion);
        private static readonly byte[] _memorySize = BitConverter.GetBytes(Constants.MemorySize / Constants.Mebibyte);
        private static readonly byte[] _iterations = BitConverter.GetBytes(Constants.Iterations);

        public static void WriteHeaders(FileStream outputFile, byte[] ephemeralPublicKey, byte[] salt, byte[] nonce, byte[] encryptedHeader)
        {
            const int offset = 0;
            outputFile.Write(_magicBytes, offset, _magicBytes.Length);
            outputFile.Write(_formatVersion, offset, _formatVersion.Length);
            outputFile.Write(ephemeralPublicKey, offset, ephemeralPublicKey.Length);
            outputFile.Write(salt, offset, salt.Length);
            outputFile.Write(nonce, offset, nonce.Length);
            outputFile.Write(encryptedHeader, offset, encryptedHeader.Length);
        }

        public static byte[] GetFileNameLength(string inputFilePath)
        {
            if (!Globals.ObfuscateFileNames) { return BitConverter.GetBytes(0); }
            string fileName = Path.GetFileName(inputFilePath);
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            return BitConverter.GetBytes(fileNameBytes.Length);
        }

        public static byte[] GetMagicBytes()
        {
            return _magicBytes;
        }

        public static byte[] GetFileFormatVersion()
        {
            return _formatVersion;
        }

        public static byte[] GetMemorySize()
        {
            return _memorySize;
        }

        public static byte[] GetIterations()
        {
            return _iterations;
        }

        public static int GetHeadersLength()
        {
            return _magicBytes.Length + _formatVersion.Length + Constants.EphemeralPublicKeyLength + Constants.SaltLength + Constants.XChaChaNonceLength + Constants.EncryptedHeaderLength;
        }

        public static byte[] ReadMagicBytes(string inputFilePath)
        {
            return FileHandling.ReadFileHeader(inputFilePath, offset: 0, _magicBytes.Length);
        }

        public static byte[] ReadFileFormatVersion(string inputFilePath)
        {
            int offset = _magicBytes.Length;
            return FileHandling.ReadFileHeader(inputFilePath, offset, _formatVersion.Length);
        }

        public static void ValidateFormatVersion(string filePath, byte[] formatVersion, byte[] currentFormatVersion)
        {
            bool validFormatVersion = Sodium.Utilities.Compare(formatVersion, currentFormatVersion);
            if (!validFormatVersion) { throw new ArgumentOutOfRangeException(filePath, "Incorrect file format for this version of Kryptor."); }
        }

        public static byte[] ReadEphemeralPublicKey(string inputFilePath)
        {
            int offset = _magicBytes.Length + _formatVersion.Length;
            return FileHandling.ReadFileHeader(inputFilePath, offset, Constants.EphemeralPublicKeyLength);
        }

        public static byte[] ReadSalt(string inputFilePath)
        {
            int offset = _magicBytes.Length + _formatVersion.Length + Constants.EphemeralPublicKeyLength;
            return FileHandling.ReadFileHeader(inputFilePath, offset, Constants.SaltLength);
        }

        public static byte[] ReadNonce(string inputFilePath)
        {
            int offset = _magicBytes.Length + _formatVersion.Length + Constants.EphemeralPublicKeyLength + Constants.SaltLength;
            return FileHandling.ReadFileHeader(inputFilePath, offset, Constants.XChaChaNonceLength);
        }

        public static byte[] ReadEncryptedHeader(string inputFilePath)
        {
            int offset = _magicBytes.Length + _formatVersion.Length + Constants.EphemeralPublicKeyLength + Constants.SaltLength + Constants.XChaChaNonceLength;
            return FileHandling.ReadFileHeader(inputFilePath, offset, Constants.EncryptedHeaderLength);
        }

        public static int GetLastChunkLength(byte[] header)
        {
            byte[] lastChunkLength = new byte[Constants.BitConverterLength];
            Array.Copy(header, Constants.KeyCommitmentBlockLength, lastChunkLength, destinationIndex: 0, lastChunkLength.Length);
            return BitConverter.ToInt32(lastChunkLength);
        }

        public static int GetFileNameLength(byte[] header)
        {
            byte[] fileNameLength = new byte[Constants.BitConverterLength];
            int sourceIndex = Constants.KeyCommitmentBlockLength + Constants.BitConverterLength;
            Array.Copy(header, sourceIndex, fileNameLength, destinationIndex: 0, fileNameLength.Length);
            return BitConverter.ToInt32(fileNameLength);
        }

        public static byte[] GetDataEncryptionKey(byte[] header)
        {
            byte[] dataEncryptionKey = new byte[Constants.EncryptionKeySize];
            int sourceIndex = header.Length - dataEncryptionKey.Length;
            Array.Copy(header, sourceIndex, dataEncryptionKey, destinationIndex: 0, dataEncryptionKey.Length);
            return dataEncryptionKey;
        }
    }
}
