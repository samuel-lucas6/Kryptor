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

        public static void SignFile(string filePath, string comment, bool preHash, byte[] privateKey)
        {
            if (!preHash) { preHash = IsPreHashingRequired(filePath); }
            byte[] preHashed = BitConverter.GetBytes(preHash);
            byte[] fileSignature = ComputeFileSignature(filePath, preHash, privateKey);
            byte[] commentBytes = Encoding.UTF8.GetBytes(comment);
            byte[] signatureFileBytes = Arrays.Concat(Constants.SignatureMagicBytes, Constants.SignatureVersion, preHashed, fileSignature, commentBytes);
            byte[] globalSignature = ComputeGlobalSignature(signatureFileBytes, privateKey);
            CreateSignatureFile(filePath, signatureFileBytes, globalSignature);
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
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
            return Blake2.Hash(fileStream);
        }

        private static byte[] ComputeGlobalSignature(byte[] signatureFileBytes, byte[] privateKey)
        {
            return PublicKeyAuth.SignDetached(signatureFileBytes, privateKey);
        }

        private static void CreateSignatureFile(string filePath, byte[] signatureFileBytes, byte[] globalSignature)
        {
            const int offset = 0;
            string signatureFilePath = filePath + Constants.SignatureExtension;
            using var signatureFile = new FileStream(signatureFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
            signatureFile.Write(signatureFileBytes, offset, signatureFileBytes.Length);
            signatureFile.Write(globalSignature, offset, globalSignature.Length);
            File.SetAttributes(signatureFilePath, FileAttributes.ReadOnly);
        }

        public static bool VerifySignature(string signatureFilePath, string filePath, byte[] publicKey, out string comment)
        {
            using var signatureFile = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
            byte[] magicBytes = GetMagicBytes(signatureFile);
            byte[] formatVersion = GetFormatVersion(signatureFile);
            FileHeaders.ValidateFormatVersion(formatVersion, Constants.SignatureVersion);
            byte[] preHashed = GetPreHashedHeader(signatureFile);
            byte[] fileSignature = GetFileSignature(signatureFile);
            byte[] commentBytes = GetCommentBytes(signatureFile);
            byte[] signatureFileBytes = Arrays.Concat(magicBytes, formatVersion, preHashed, fileSignature, commentBytes);
            bool validGlobalSignature = VerifyGlobalSignature(signatureFile, signatureFileBytes, publicKey);
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

        private static byte[] GetMagicBytes(FileStream signatureFile)
        {
            return FileHandling.ReadFileHeader(signatureFile, Constants.SignatureMagicBytes.Length);
        }

        private static byte[] GetFormatVersion(FileStream signatureFile)
        {
            return FileHandling.ReadFileHeader(signatureFile, Constants.SignatureVersion.Length);
        }

        private static byte[] GetPreHashedHeader(FileStream signatureFile)
        {
            return FileHandling.ReadFileHeader(signatureFile, _preHashedHeaderLength);
        }

        private static byte[] GetFileSignature(FileStream signatureFile)
        {
            return FileHandling.ReadFileHeader(signatureFile, Constants.SignatureLength);
        }

        private static byte[] GetCommentBytes(FileStream signatureFile)
        {
            int offset = Constants.SignatureMagicBytes.Length + Constants.SignatureVersion.Length + _preHashedHeaderLength + Constants.SignatureLength;
            int length = (int)(signatureFile.Length - offset - Constants.SignatureLength);
            return FileHandling.ReadFileHeader(signatureFile, length);
        }

        private static bool VerifyGlobalSignature(FileStream signatureFile, byte[] signatureFileBytes, byte[] publicKey)
        {
            byte[] globalSignature = GetGlobalSignature(signatureFile);
            return PublicKeyAuth.VerifyDetached(globalSignature, signatureFileBytes, publicKey);
        }

        private static byte[] GetGlobalSignature(FileStream signatureFile)
        {
            return FileHandling.ReadFileHeader(signatureFile, Constants.SignatureLength);
        }
    }
}
