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
        public static bool SignFile(string encryptedFilePath, byte[] macKey)
        {
            byte[] fileHash = ComputeFileHash(encryptedFilePath, macKey);
            return AppendHash(encryptedFilePath, fileHash);
        }

        private static byte[] ComputeFileHash(string encryptedFilePath, byte[] macKey)
        {
            try
            {
                byte[] computedHash = new byte[Constants.HashLength];
                using (var fileStream = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    MemoryEncryption.DecryptByteArray(ref macKey);
                    computedHash = HashingAlgorithms.Blake2(fileStream, macKey);
                    MemoryEncryption.EncryptByteArray(ref macKey);
                }
                return computedHash;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(encryptedFilePath, ex.GetType().Name, "Unable to compute MAC.");
                return null;
            }
        }

        public static bool AuthenticateFile(string filePath, byte[] macKey, out byte[] storedHash)
        {
            try
            {
                bool tampered = true;
                storedHash = ReadStoredHash(filePath);
                if (storedHash != null)
                {
                    byte[] computedHash = new byte[Constants.HashLength];
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                    {
                        // Remove the stored MAC from the file before computing the MAC
                        fileStream.SetLength(fileStream.Length - computedHash.Length);
                        MemoryEncryption.DecryptByteArray(ref macKey);
                        computedHash = HashingAlgorithms.Blake2(fileStream, macKey);
                        MemoryEncryption.EncryptByteArray(ref macKey);
                    }
                    // Invert result
                    tampered = !Sodium.Utilities.Compare(storedHash, computedHash);
                    if (tampered == true)
                    {
                        // Restore the stored MAC
                        AppendHash(filePath, storedHash);
                    }
                }
                return tampered;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to authenticate the file.");
                storedHash = null;
                return true;
            }
        }

        private static byte[] ReadStoredHash(string filePath)
        {
            try
            {
                byte[] storedHash = new byte[Constants.HashLength];
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
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to read the MAC stored in the file.");
                return null;
            }
        }

        public static bool AppendHash(string filePath, byte[] fileHash)
        {
            try
            {
                NullChecks.ByteArray(fileHash);
                using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    fileStream.Write(fileHash, 0, fileHash.Length);
                }
                return true;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to append the MAC to the file. This data is required for decryption of the file.");
                return false;
            }
        }
    }
}
