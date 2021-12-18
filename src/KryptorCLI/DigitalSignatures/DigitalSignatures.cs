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
using System.Text;
using Sodium;

namespace KryptorCLI;

public static class DigitalSignatures
{
    private const int PreHashedHeaderLength = 1;

    public static void SignFile(string filePath, string signatureFilePath, string comment, bool preHash, byte[] privateKey)
    {
        if (!preHash) { preHash = IsPreHashingRequired(filePath); }
        byte[] preHashed = BitConverter.GetBytes(preHash);
        byte[] fileBytes = GetFileBytes(filePath, preHash);
        byte[] fileSignature = PublicKeyAuth.SignDetached(fileBytes, privateKey);
        byte[] commentBytes = Encoding.UTF8.GetBytes(comment);
        byte[] signatureFileBytes = Arrays.Concat(Constants.SignatureMagicBytes, Constants.SignatureVersion, preHashed, fileSignature, commentBytes);
        byte[] globalSignature = PublicKeyAuth.SignDetached(signatureFileBytes, privateKey);
        CreateSignatureFile(filePath, signatureFilePath, signatureFileBytes, globalSignature);
    }

    private static bool IsPreHashingRequired(string filePath)
    {
        int oneGibibyte = Constants.Mebibyte * 1024;
        long fileSize = FileHandling.GetFileLength(filePath);
        return fileSize >= oneGibibyte;
    }

    private static byte[] GetFileBytes(string filePath, bool preHash)
    {
        if (!preHash) { return File.ReadAllBytes(filePath); }
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
        return Blake2b.Hash(fileStream);
    }

    private static void CreateSignatureFile(string filePath, string signatureFilePath, byte[] signatureFileBytes, byte[] globalSignature)
    {
        if (string.IsNullOrEmpty(signatureFilePath)) { signatureFilePath = filePath + Constants.SignatureExtension; }
        if (File.Exists(signatureFilePath)) { File.SetAttributes(signatureFilePath, FileAttributes.Normal); }
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
        FileHeaders.ValidateFormatVersion(formatVersion, Constants.SignatureVersion);
        byte[] preHashed = FileHandling.ReadFileHeader(signatureFile, PreHashedHeaderLength);
        byte[] fileSignature = FileHandling.ReadFileHeader(signatureFile, Constants.SignatureLength);
        byte[] commentBytes = GetCommentBytes(signatureFile);
        byte[] signatureFileBytes = Arrays.Concat(magicBytes, formatVersion, preHashed, fileSignature, commentBytes);
        byte[] globalSignature = FileHandling.ReadFileHeader(signatureFile, Constants.SignatureLength);
        bool validGlobalSignature = PublicKeyAuth.VerifyDetached(globalSignature, signatureFileBytes, publicKey);
        if (!validGlobalSignature)
        {
            comment = string.Empty;
            return false;
        }
        bool preHash = BitConverter.ToBoolean(preHashed);
        byte[] fileBytes = GetFileBytes(filePath, preHash);
        comment = Encoding.UTF8.GetString(commentBytes);
        return PublicKeyAuth.VerifyDetached(fileSignature, fileBytes, publicKey);
    }

    private static byte[] GetCommentBytes(FileStream signatureFile)
    {
        int offset = Constants.SignatureMagicBytes.Length + Constants.SignatureVersion.Length + PreHashedHeaderLength + Constants.SignatureLength;
        int length = (int)(signatureFile.Length - offset - Constants.SignatureLength);
        return FileHandling.ReadFileHeader(signatureFile, length);
    }
}