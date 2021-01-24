using System;
using System.IO;
using System.Text;
using Sodium;

/*
    Kryptor: Modern and secure file encryption.
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
    public static class DigitalSignatures
    {
        private static readonly byte[] _magicBytes = Encoding.UTF8.GetBytes(Constants.SignatureMagicBytes);
        private const int _algorithmHeaderLength = 1;
        private static byte[] _commentBytes;

        public static void SignFile(string filePath, string comment, bool preHashed, byte[] privateKey)
        {
            byte[] algorithmHeader = BitConverter.GetBytes(preHashed);
            byte[] fileSignature = ComputeFileSignature(filePath, preHashed, privateKey);
            byte[] commentBytes = Encoding.UTF8.GetBytes(comment);
            // Sign the entire signature file
            byte[] signatureFileBytes = Utilities.ConcatArrays(_magicBytes, algorithmHeader, fileSignature, commentBytes);
            byte[] globalSignature = ComputeGlobalSignature(signatureFileBytes, privateKey);
            CreateSignatureFile(filePath, algorithmHeader, fileSignature, commentBytes, globalSignature);
        }

        private static byte[] ComputeFileSignature(string filePath, bool preHashed, byte[] privateKey)
        {
            byte[] fileBytes = GetFileBytes(filePath, preHashed);
            return PublicKeyAuth.SignDetached(fileBytes, privateKey);
        }

        private static byte[] GetFileBytes(string filePath, bool preHashed)
        {
            int oneGibibyte = 1024 * Constants.Mebibyte;
            long fileSize = FileHandling.GetFileLength(filePath);
            if (fileSize >= oneGibibyte || preHashed)
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan);
                return Blake2.Hash(fileStream);
            }
            return File.ReadAllBytes(filePath);
        }

        private static byte[] ComputeGlobalSignature(byte[] signatureFileBytes, byte[] privateKey)
        {
            return PublicKeyAuth.SignDetached(signatureFileBytes, privateKey);
        }

        private static void CreateSignatureFile(string filePath, byte[] algorithmHeader, byte[] fileSignature, byte[] commentBytes, byte[] globalSignature)
        {
            const int offset = 0;   
            string signatureFilePath = filePath + Constants.SignatureExtension;
            using var signatureFile = new FileStream(signatureFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan);
            signatureFile.Write(_magicBytes, offset, _magicBytes.Length);
            signatureFile.Write(algorithmHeader, offset, algorithmHeader.Length);
            signatureFile.Write(fileSignature, offset, fileSignature.Length);
            signatureFile.Write(commentBytes, offset, commentBytes.Length);
            signatureFile.Write(globalSignature, offset, globalSignature.Length);
            File.SetAttributes(signatureFilePath, FileAttributes.ReadOnly);
        }

        public static byte[] GetSignatureMagicBytes()
        {
            return _magicBytes;
        }

        public static bool VerifySignature(string signatureFilePath, string filePath, byte[] publicKey)
        {
            using var signatureFile = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileBufferSize, FileOptions.RandomAccess);
            // Verify the global signature
            byte[] magicBytes = GetMagicBytes(signatureFile);
            byte[] algorithm = GetAlgorithm(signatureFile);
            byte[] fileSignature = GetFileSignature(signatureFile);
            _commentBytes = GetCommentBytes(signatureFile);
            byte[] signatureFileBytes = Utilities.ConcatArrays(magicBytes, algorithm, fileSignature, _commentBytes);
            bool validGlobalSignature = VerifyGlobalSignature(signatureFile, signatureFileBytes, publicKey);
            if (!validGlobalSignature) { return false; }
            // Verify the file signature
            bool preHashed = BitConverter.ToBoolean(algorithm);
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
            return FileHandling.ReadFileHeader(signatureFile, offset: 0, _magicBytes.Length);
        }

        private static byte[] GetAlgorithm(FileStream signatureFile)
        {
            return FileHandling.ReadFileHeader(signatureFile, _magicBytes.Length, _algorithmHeaderLength);
        }

        private static byte[] GetFileSignature(FileStream signatureFile)
        {
            int offset = _magicBytes.Length + _algorithmHeaderLength;
            return FileHandling.ReadFileHeader(signatureFile, offset, Constants.SignatureLength);
        }

        private static byte[] GetCommentBytes(FileStream signatureFile)
        {
            int offset = _magicBytes.Length + _algorithmHeaderLength + Constants.SignatureLength;
            int length = (int)(signatureFile.Length - (Constants.SignatureLength * 2) - _algorithmHeaderLength - _magicBytes.Length);
            return FileHandling.ReadFileHeader(signatureFile, offset, length);
        }

        public static string GetComment()
        {
            return Encoding.UTF8.GetString(_commentBytes);
        }
    }
}
