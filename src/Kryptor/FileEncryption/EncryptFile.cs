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
using System.Buffers.Binary;
using System.Security.Cryptography;
using Geralt;
using ChaCha20BLAKE2;

namespace Kryptor;

public static class EncryptFile
{
    public static void Encrypt(string inputFilePath, string outputFilePath, bool directory, byte[] ephemeralPublicKey, byte[] salt, byte[] keyEncryptionKey)
    {
        var dataEncryptionKey = GC.AllocateArray<byte>(Constants.EncryptionKeyLength, pinned: true);
        SecureRandom.Fill(dataEncryptionKey);
        try
        {
            using (var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan))
            using (var outputFile = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan))
            {
                var nonce = new byte[Constants.XChaChaNonceLength];
                SecureRandom.Fill(nonce);
                byte[] encryptedHeader = EncryptFileHeader(inputFilePath, directory, ephemeralPublicKey, dataEncryptionKey, nonce, keyEncryptionKey);
                FileHeaders.WriteHeaders(outputFile, ephemeralPublicKey, salt, nonce, encryptedHeader);
                ConstantTime.Increment(nonce);
                EncryptChunks(inputFile, outputFile, nonce, dataEncryptionKey);
                CryptographicOperations.ZeroMemory(dataEncryptionKey);
            }
            Globals.SuccessfulCount += 1;
            if (Globals.Overwrite) {
                FileHandling.OverwriteFile(inputFilePath, outputFilePath);
            }
            else if (directory) {
                FileHandling.DeleteFile(inputFilePath);
            }
            FileHandling.SetFileAttributesReadOnly(outputFilePath);
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            CryptographicOperations.ZeroMemory(dataEncryptionKey);
            FileHandling.DeleteFile(outputFilePath);
            throw;
        }
    }

    private static byte[] EncryptFileHeader(string inputFilePath, bool directory, byte[] ephemeralPublicKey, byte[] dataEncryptionKey, byte[] nonce, byte[] keyEncryptionKey)
    {
        long fileLength = FileHandling.GetFileLength(inputFilePath);
        var paddingLength = new byte[Constants.IntBytesLength];
        if (fileLength == 0) {
            BinaryPrimitives.WriteInt32LittleEndian(paddingLength, Constants.FileChunkSize);
        }
        else {
            int lastChunkRemainder = Convert.ToInt32(fileLength % Constants.FileChunkSize);
            BinaryPrimitives.WriteInt32LittleEndian(paddingLength, lastChunkRemainder == 0 ? lastChunkRemainder : Constants.FileChunkSize - lastChunkRemainder);
        }
        byte[] isDirectory = BitConverter.GetBytes(directory);
        byte[] fileName = Encoding.UTF8.GetBytes(Path.GetFileName(inputFilePath));
        var paddedFileName = new byte[Constants.FileNameHeaderLength];
        if (Globals.EncryptFileNames) {
            Array.Copy(fileName, paddedFileName, fileName.Length);
        }
        var fileNameLength = new byte[Constants.IntBytesLength];
        BinaryPrimitives.WriteInt32LittleEndian(fileNameLength, !Globals.EncryptFileNames ? 0 : fileName.Length);
        byte[] fileHeader = Arrays.Concat(paddingLength, isDirectory, fileNameLength, paddedFileName, dataEncryptionKey);
        long chunkCount = (long)Math.Ceiling((double)(fileLength != 0 ? fileLength : 1) / Constants.FileChunkSize);
        var ciphertextLength = new byte[Constants.LongBytesLength];
        BinaryPrimitives.WriteInt64LittleEndian(ciphertextLength, chunkCount * Constants.CiphertextChunkLength);
        byte[] additionalData = Arrays.Concat(ciphertextLength, Constants.EncryptionMagicBytes, Constants.EncryptionVersion, ephemeralPublicKey);
        fileHeader = XChaCha20BLAKE2b.Encrypt(fileHeader, nonce, keyEncryptionKey, additionalData);
        return fileHeader;
    }

    private static void EncryptChunks(Stream inputFile, Stream outputFile, byte[] nonce, byte[] dataEncryptionKey)
    {
        var plaintextChunk = GC.AllocateArray<byte>(Constants.FileChunkSize, pinned: true);
        if (inputFile.Length == 0) {
            byte[] ciphertextChunk = XChaCha20BLAKE2b.Encrypt(plaintextChunk, nonce, dataEncryptionKey);
            outputFile.Write(ciphertextChunk, offset: 0, ciphertextChunk.Length);
            return;
        }
        while (inputFile.Read(plaintextChunk, offset: 0, plaintextChunk.Length) > 0)
        {
            byte[] ciphertextChunk = XChaCha20BLAKE2b.Encrypt(plaintextChunk, nonce, dataEncryptionKey);
            ConstantTime.Increment(nonce);
            outputFile.Write(ciphertextChunk, offset: 0, ciphertextChunk.Length);
        }
        CryptographicOperations.ZeroMemory(plaintextChunk);
    }
}