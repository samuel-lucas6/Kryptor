using System;
using System.IO;

/*  
    Kryptor: Free and open source file encryption software.
    Copyright(C) 2020 Samuel Lucas

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace Kryptor
{
    public static class FileAuthentication
    {
        public static bool SignFile(string encryptedFilePath, byte[] hmacKey)
        {
            byte[] fileHash = ComputeFileHash(encryptedFilePath, hmacKey);
            return AppendHash(encryptedFilePath, fileHash);
        }

        private static byte[] ComputeFileHash(string encryptedFilePath, byte[] hmacKey)
        {
            try
            {
                byte[] computedHash = new byte[Constants.HMACLength];
                MemoryEncryption.DecryptByteArray(ref hmacKey);
                using (var fileStream = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    computedHash = HashingAlgorithms.HMAC(fileStream, hmacKey);
                }
                MemoryEncryption.EncryptByteArray(ref hmacKey);
                return computedHash;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(encryptedFilePath, ex.GetType().Name, "Unable to compute HMAC.");
                return null;
            }
        }

        public static bool AuthenticateFile(string filePath, byte[] hmacKey)
        {
            try
            {
                bool tampered = true;
                byte[] storedHash = ReadStoredHash(filePath);
                byte[] computedHash = new byte[Constants.HMACLength];
                if (storedHash != null)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                    {
                        fileStream.Seek(0, SeekOrigin.Begin);
                        // Remove the stored HMAC from the file before computing the HMAC
                        fileStream.SetLength(fileStream.Length - computedHash.Length);
                        MemoryEncryption.DecryptByteArray(ref hmacKey);
                        computedHash = HashingAlgorithms.HMAC(fileStream, hmacKey);
                    }
                    MemoryEncryption.EncryptByteArray(ref hmacKey);
                    tampered = !Sodium.Utilities.Compare(storedHash, computedHash);
                    // Restore the stored HMAC
                    AppendHash(filePath, storedHash);
                }
                return tampered;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to authenticate the file.");
                return true;
            }
        }

        private static byte[] ReadStoredHash(string filePath)
        {
            try
            {
                byte[] storedHash = new byte[Constants.HMACLength];
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Read the last 64 bytes of the file
                    fileStream.Seek(fileStream.Length - storedHash.Length, SeekOrigin.Begin);
                    fileStream.Read(storedHash, 0, storedHash.Length);
                }
                return storedHash;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to read the HMAC stored in the file.");
                return null;
            }
        }

        private static bool AppendHash(string filePath, byte[] fileHash)
        {
            try
            {
                NullChecks.ByteArray(fileHash);
                using (var fsAppend = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    fsAppend.Write(fileHash, 0, fileHash.Length);
                }
                return true;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to append HMAC to the file. This data is required for decryption of the file. The original file will not be overwritten/deleted.");
                return false;
            }
        }
    }
}
