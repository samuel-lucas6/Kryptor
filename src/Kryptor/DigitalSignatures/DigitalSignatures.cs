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
            prehash = new FileInfo(filePath).Length >= 1073741824;
        }
        Span<byte> prehashed = BitConverter.GetBytes(prehash);
        Span<byte> fileBytes = GetFileBytes(filePath, prehash);
        Span<byte> fileSignature = stackalloc byte[Ed25519.SignatureSize];
        Ed25519.Sign(fileSignature, fileBytes, privateKey);
        
        Span<byte> commentBytes = Encoding.UTF8.GetBytes(comment);
        Span<byte> signatureFileBytes = new byte[Constants.SignatureMagicBytes.Length + Constants.SignatureVersion.Length + prehashed.Length + fileSignature.Length + commentBytes.Length];
        Spans.Concat(signatureFileBytes, Constants.SignatureMagicBytes, Constants.SignatureVersion, prehashed, fileSignature, commentBytes);
        
        Span<byte> globalSignature = stackalloc byte[Ed25519.SignatureSize];
        Ed25519.Sign(globalSignature, signatureFileBytes, privateKey);
        
        CreateSignatureFile(filePath, signatureFilePath, signatureFileBytes, globalSignature);
    }

    private static Span<byte> GetFileBytes(string filePath, bool prehash)
    {
        if (!prehash) {
            return File.ReadAllBytes(filePath);
        }
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
        using var blake2b = new BLAKE2bHashAlgorithm(BLAKE2b.MaxHashSize);
        return blake2b.ComputeHash(fileStream);
    }

    private static void CreateSignatureFile(string filePath, string signatureFilePath, Span<byte> signatureFileBytes, Span<byte> globalSignature)
    {
        if (string.IsNullOrEmpty(signatureFilePath)) {
            signatureFilePath = filePath + Constants.SignatureExtension;
        }
        if (File.Exists(signatureFilePath)) {
            File.SetAttributes(signatureFilePath, FileAttributes.Normal);
        }
        using var signatureFile = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, Constants.DefaultFileStreamBufferSize, FileOptions.SequentialScan);
        signatureFile.Write(signatureFileBytes);
        signatureFile.Write(globalSignature);
        File.SetAttributes(signatureFilePath, FileAttributes.ReadOnly);
    }

    public static bool VerifySignature(string signatureFilePath, string filePath, Span<byte> publicKey, out string comment)
    {
        Span<byte> signatureFileBytes = File.ReadAllBytes(signatureFilePath);
        
        Span<byte> globalSignature = signatureFileBytes[^Ed25519.SignatureSize..];
        if (!Ed25519.Verify(globalSignature, signatureFileBytes[..^globalSignature.Length], publicKey)) {
            comment = string.Empty;
            return false;
        }
        
        Span<byte> prehashed = signatureFileBytes.Slice(Constants.SignatureMagicBytes.Length + Constants.SignatureVersion.Length, Constants.BoolBytesLength);
        bool prehash = BitConverter.ToBoolean(prehashed);
        Span<byte> fileBytes = GetFileBytes(filePath, prehash);
        Span<byte> fileSignature = signatureFileBytes.Slice(Constants.SignatureMagicBytes.Length + Constants.SignatureVersion.Length + prehashed.Length, Ed25519.SignatureSize);

        Span<byte> commentBytes = signatureFileBytes[(Constants.SignatureMagicBytes.Length + Constants.SignatureVersion.Length + prehashed.Length + fileSignature.Length)..^globalSignature.Length];
        comment = Encoding.UTF8.GetString(commentBytes);
        return Ed25519.Verify(fileSignature, fileBytes, publicKey);
    }
}