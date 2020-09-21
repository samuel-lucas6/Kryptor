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
    public static class DecryptionBackup
    {
        public static void BackupTrailers(string filePath, string backupFilePath, int trailersLength)
        {
            byte[] trailers = ReadTrailers(filePath, trailersLength);
            using (var backupFile = new FileStream(backupFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                backupFile.Write(trailers, 0, trailers.Length);
            }
            File.SetAttributes(backupFilePath, FileAttributes.ReadOnly);
        }

        private static byte[] ReadTrailers(string filePath, int trailersLength)
        {
            byte[] trailers = new byte[trailersLength];
            using (var encryptedFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                encryptedFile.Seek(encryptedFile.Length - trailersLength, SeekOrigin.Begin);
                encryptedFile.Read(trailers, 0, trailers.Length);
            }
            return trailers;
        }

        public static void RestoreTrailers(string filePath, string backupFilePath)
        {
            try
            {
                if (File.Exists(backupFilePath))
                {
                    File.SetAttributes(backupFilePath, FileAttributes.Normal);
                    byte[] trailers = ReadBackupFile(backupFilePath);
                    using (var encryptedFile = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                    {
                        encryptedFile.Write(trailers, 0, trailers.Length);
                    }
                    File.Delete(backupFilePath);
                    File.SetAttributes(filePath, FileAttributes.ReadOnly);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to restore trailers from backup file.");
            }
        }

        private static byte[] ReadBackupFile(string backupFilePath)
        {
            using (var backupFile = new FileStream(backupFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int trailersLength = Convert.ToInt32(backupFile.Length);
                byte[] trailers = new byte[trailersLength];
                backupFile.Read(trailers, 0, trailersLength);
                return trailers;
            }
        }
    }
}
