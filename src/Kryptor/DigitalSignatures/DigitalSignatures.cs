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

public static class DigitalSignatures
{
    public static void SignFile(string filePath, string signatureFilePath, string comment, bool prehash, Span<byte> privateKey)
    {
        if (!prehash) {
            prehash = FileHandling.GetFileLength(filePath) >= Constants.Mebibyte * 1024;
        }
        byte[] prehashed = BitConverter.GetBytes(prehash);
        byte[] fileBytes = GetFileBytes(filePath, prehash);
        var fileSignature = new byte[Ed25519.SignatureSize];
        Ed25519.Sign(fileSignature, fileBytes, privateKey);
        byte[] commentBytes = Encoding.UTF8.GetBytes(comment);
        byte[] signatureFileBytes = Arrays.Concat(Constants.SignatureMagicBytes, Constants.SignatureVersion, prehashed, fileSignature, commentBytes);
        var globalSignature = new byte[Ed25519.SignatureSize];
        Ed25519.Sign(globalSignature, signatureFileBytes, privateKey);
        CreateSignatureFile(filePath, signatureFilePath, signatureFileBytes, globalSignature);
        Globals.SuccessfulCount++;
    }

    private static byte[] GetFileBytes(string filePath, bool prehash)
    {
        if (!prehash) {
            return File.ReadAllBytes(filePath);
        }
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
        using var blake2b = new BLAKE2bHashAlgorithm(BLAKE2b.MaxHashSize);
        return blake2b.ComputeHash(fileStream);
    }

    private static void CreateSignatureFile(string filePath, string signatureFilePath, byte[] signatureFileBytes, byte[] globalSignature)
    {
        if (string.IsNullOrEmpty(signatureFilePath)) {
            signatureFilePath = filePath + Constants.SignatureExtension;
        }
        if (File.Exists(signatureFilePath)) {
            File.SetAttributes(signatureFilePath, FileAttributes.Normal);
        }
        using var signatureFile = new FileStream(signatureFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
        signatureFile.Write(signatureFileBytes, offset: 0, signatureFileBytes.Length);
        signatureFile.Write(globalSignature, offset: 0, globalSignature.Length);
        File.SetAttributes(signatureFilePath, FileAttributes.ReadOnly);
    }

    public static bool VerifySignature(string signatureFilePath, string filePath, byte[] publicKey, out string comment)
    {
        using var signatureFile = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
        byte[] magicBytes = FileHandling.ReadFileHeader(signatureFile, Constants.SignatureMagicBytes.Length);
        byte[] formatVersion = FileHandling.ReadFileHeader(signatureFile, Constants.SignatureVersion.Length);
        byte[] prehashed = FileHandling.ReadFileHeader(signatureFile, Constants.BoolBytesLength);
        byte[] fileSignature = FileHandling.ReadFileHeader(signatureFile, Constants.SignatureLength);
        byte[] commentBytes = FileHandling.ReadFileHeader(signatureFile, (int)(signatureFile.Length - magicBytes.Length - formatVersion.Length - prehashed.Length - fileSignature.Length - Constants.SignatureLength));
        byte[] signatureFileBytes = Arrays.Concat(magicBytes, formatVersion, prehashed, fileSignature, commentBytes);
        byte[] globalSignature = FileHandling.ReadFileHeader(signatureFile, Constants.SignatureLength);
        bool validGlobalSignature = Ed25519.Verify(globalSignature, signatureFileBytes, publicKey);
        if (!validGlobalSignature) {
            comment = string.Empty;
            return false;
        }
        bool prehash = BitConverter.ToBoolean(prehashed);
        byte[] fileBytes = GetFileBytes(filePath, prehash);
        comment = Encoding.UTF8.GetString(commentBytes);
        return Ed25519.Verify(fileSignature, fileBytes, publicKey);
    }
}