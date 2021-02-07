using System;
using System.IO;
using System.Text;
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
    public static class DigitalSignatures
    {
        private const int _preHashedHeaderLength = 1;
        private static byte[] _commentBytes;

        public static void SignFile(string filePath, string comment, bool preHash, byte[] privateKey)
        {
            if (!preHash) { preHash = IsPreHashingRequired(filePath); }
            byte[] preHashedHeader = BitConverter.GetBytes(preHash);
            byte[] fileSignature = ComputeFileSignature(filePath, preHash, privateKey);
            byte[] commentBytes = Encoding.UTF8.GetBytes(comment);
            // Sign the entire signature file
            byte[] signatureFileBytes = Arrays.Concat(Constants.SignatureMagicBytes, Constants.SignatureVersion, preHashedHeader, fileSignature, commentBytes);
            byte[] globalSignature = ComputeGlobalSignature(signatureFileBytes, privateKey);
            CreateSignatureFile(filePath, preHashedHeader, fileSignature, commentBytes, globalSignature);
        }

        private static bool IsPreHashingRequired(string filePath)
        {
            int oneGibibyte = 1024 * Constants.Mebibyte;
            long fileSize = FileHandling.GetFileLength(filePath);
            return fileSize >= oneGibibyte;
        }

        private static byte[] ComputeFileSignature(string filePath, bool preHash, byte[] privateKey)
        {
            byte[] fileBytes = GetFileBytes(filePath, preHash);
            return PublicKeyAuth.SignDetached(fileBytes, privateKey);
        }

        private static byte[] GetFileBytes(string filePath, bool preHash)
        {
            if (!preHash) { return File.ReadAllBytes(filePath); }
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
            return Blake2.Hash(fileStream);
        }

        private static byte[] ComputeGlobalSignature(byte[] signatureFileBytes, byte[] privateKey)
        {
            return PublicKeyAuth.SignDetached(signatureFileBytes, privateKey);
        }

        private static void CreateSignatureFile(string filePath, byte[] algorithmHeader, byte[] fileSignature, byte[] commentBytes, byte[] globalSignature)
        {
            const int offset = 0;   
            string signatureFilePath = filePath + Constants.SignatureExtension;
            using var signatureFile = new FileStream(signatureFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
            signatureFile.Write(Constants.SignatureMagicBytes, offset, Constants.SignatureMagicBytes.Length);
            signatureFile.Write(Constants.SignatureVersion, offset, Constants.SignatureVersion.Length);
            signatureFile.Write(algorithmHeader, offset, algorithmHeader.Length);
            signatureFile.Write(fileSignature, offset, fileSignature.Length);
            signatureFile.Write(commentBytes, offset, commentBytes.Length);
            signatureFile.Write(globalSignature, offset, globalSignature.Length);
            File.SetAttributes(signatureFilePath, FileAttributes.ReadOnly);
        }

        public static bool VerifySignature(string signatureFilePath, string filePath, byte[] publicKey)
        {
            using var signatureFile = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
            // Verify the global signature
            byte[] magicBytes = GetMagicBytes(signatureFile);
            byte[] formatVersion = GetFormatVersion(signatureFile);
            FileHeaders.ValidateFormatVersion(signatureFilePath, formatVersion, Constants.SignatureVersion);
            byte[] preHashedHeader = GetPreHashedHeader(signatureFile);
            byte[] fileSignature = GetFileSignature(signatureFile);
            _commentBytes = GetCommentBytes(signatureFile);
            byte[] signatureFileBytes = Arrays.Concat(magicBytes, formatVersion, preHashedHeader, fileSignature, _commentBytes);
            bool validGlobalSignature = VerifyGlobalSignature(signatureFile, signatureFileBytes, publicKey);
            if (!validGlobalSignature) { return false; }
            // Verify the file signature
            bool preHashed = BitConverter.ToBoolean(preHashedHeader);
            byte[] fileBytes = GetFileBytes(filePath, preHashed);
            return PublicKeyAuth.VerifyDetached(fileSignature, fileBytes, publicKey);
        }

        private static bool VerifyGlobalSignature(FileStream signatureFile, byte[] fileBytesToVerify, byte[] publicKey)
        {
            byte[] globalSignature = GetGlobalSignature(signatureFile);
            return PublicKeyAuth.VerifyDetached(globalSignature, fileBytesToVerify, publicKey);
        }

        private static byte[] GetGlobalSignature(FileStream signatureFile)
        {
            long offset = signatureFile.Length - Constants.SignatureLength;
            return FileHandling.ReadFileHeader(signatureFile, offset, Constants.SignatureLength);
        }

        private static byte[] GetMagicBytes(FileStream signatureFile)
        {
            return FileHandling.ReadFileHeader(signatureFile, offset: 0, Constants.SignatureMagicBytes.Length);
        }

        private static byte[] GetFormatVersion(FileStream signatureFile)
        {
            return FileHandling.ReadFileHeader(signatureFile, Constants.SignatureMagicBytes.Length, Constants.SignatureVersion.Length);
        }

        private static byte[] GetPreHashedHeader(FileStream signatureFile)
        {
            int offset = Constants.SignatureMagicBytes.Length + Constants.SignatureVersion.Length;
            return FileHandling.ReadFileHeader(signatureFile, offset, _preHashedHeaderLength);
        }

        private static byte[] GetFileSignature(FileStream signatureFile)
        {
            int offset = Constants.SignatureMagicBytes.Length + Constants.SignatureVersion.Length + _preHashedHeaderLength;
            return FileHandling.ReadFileHeader(signatureFile, offset, Constants.SignatureLength);
        }

        private static byte[] GetCommentBytes(FileStream signatureFile)
        {
            int offset = Constants.SignatureMagicBytes.Length + Constants.SignatureVersion.Length + _preHashedHeaderLength + Constants.SignatureLength;
            int length = (int)(signatureFile.Length - offset - Constants.SignatureLength);
            return FileHandling.ReadFileHeader(signatureFile, offset, length);
        }

        public static string GetComment()
        {
            return Encoding.UTF8.GetString(_commentBytes);
        }
    }
}
