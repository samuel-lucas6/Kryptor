using System;
using System.IO;
using System.Text;
using Sodium;

/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2021 Samuel Lucas

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
        public static void WriteHeaders(FileStream outputFile, byte[] ephemeralPublicKey, byte[] salt, byte[] nonce, byte[] encryptedHeader)
        {
            const int offset = 0;
            outputFile.Write(Constants.KryptorMagicBytes, offset, Constants.KryptorMagicBytes.Length);
            outputFile.Write(Constants.EncryptionVersion, offset, Constants.EncryptionVersion.Length);
            outputFile.Write(ephemeralPublicKey, offset, ephemeralPublicKey.Length);
            outputFile.Write(salt, offset, salt.Length);
            outputFile.Write(nonce, offset, nonce.Length);
            outputFile.Write(encryptedHeader, offset, encryptedHeader.Length);
        }

        public static byte[] GetFileNameLength(string inputFilePath)
        {
            if (!Globals.ObfuscateFileNames) { return BitConversion.GetBytes(0); }
            string fileName = Path.GetFileName(inputFilePath);
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            return BitConversion.GetBytes(fileNameBytes.Length);
        }

        public static byte[] ReadMagicBytes(string inputFilePath)
        {
            return FileHandling.ReadFileHeader(inputFilePath, offset: 0, Constants.KryptorMagicBytes.Length);
        }

        public static byte[] ReadMagicBytes(FileStream inputFile)
        {
            return FileHandling.ReadFileHeader(inputFile, offset: 0, Constants.KryptorMagicBytes.Length);
        }

        public static byte[] ReadFileFormatVersion(FileStream inputFile)
        {
            int offset = Constants.KryptorMagicBytes.Length;
            return FileHandling.ReadFileHeader(inputFile, offset, Constants.EncryptionVersion.Length);
        }

        public static void ValidateFormatVersion(byte[] formatVersion, byte[] currentFormatVersion)
        {
            bool validFormatVersion = Utilities.Compare(formatVersion, currentFormatVersion);
            if (!validFormatVersion) { throw new ArgumentException("Incorrect file format for this version of Kryptor."); }
        }

        public static byte[] ReadEphemeralPublicKey(FileStream inputFile)
        {
            int offset = Constants.KryptorMagicBytes.Length + Constants.EncryptionVersion.Length;
            return FileHandling.ReadFileHeader(inputFile, offset, Constants.EphemeralPublicKeyLength);
        }

        public static byte[] ReadSalt(FileStream inputFile)
        {
            int offset = Constants.KryptorMagicBytes.Length + Constants.EncryptionVersion.Length + Constants.EphemeralPublicKeyLength;
            return FileHandling.ReadFileHeader(inputFile, offset, Constants.SaltLength);
        }

        public static byte[] ReadNonce(FileStream inputFile)
        {
            int offset = Constants.KryptorMagicBytes.Length + Constants.EncryptionVersion.Length + Constants.EphemeralPublicKeyLength + Constants.SaltLength;
            return FileHandling.ReadFileHeader(inputFile, offset, Constants.XChaChaNonceLength);
        }

        public static byte[] ReadEncryptedHeader(FileStream inputFile)
        {
            int offset = Constants.KryptorMagicBytes.Length + Constants.EncryptionVersion.Length + Constants.EphemeralPublicKeyLength + Constants.SaltLength + Constants.XChaChaNonceLength;
            return FileHandling.ReadFileHeader(inputFile, offset, Constants.EncryptedHeaderLength);
        }

        public static int GetLastChunkLength(byte[] header)
        {
            byte[] lastChunkLength = new byte[Constants.IntBitConverterLength];
            Array.Copy(header, lastChunkLength, lastChunkLength.Length);
            return BitConversion.ToInt32(lastChunkLength);
        }

        public static int GetFileNameLength(byte[] header)
        {
            byte[] fileNameLength = new byte[Constants.IntBitConverterLength];
            Array.Copy(header, Constants.IntBitConverterLength, fileNameLength, destinationIndex: 0, fileNameLength.Length);
            return BitConversion.ToInt32(fileNameLength);
        }

        public static byte[] GetDataEncryptionKey(byte[] header)
        {
            byte[] dataEncryptionKey = new byte[Constants.EncryptionKeyLength];
            int sourceIndex = header.Length - dataEncryptionKey.Length;
            Array.Copy(header, sourceIndex, dataEncryptionKey, destinationIndex: 0, dataEncryptionKey.Length);
            return dataEncryptionKey;
        }
    }
}
