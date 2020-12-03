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
    public static class FileHandling
    {
        public static bool? IsDirectory(string filePath)
        {
            try
            {
                var fileAttributes = File.GetAttributes(filePath);
                return fileAttributes.HasFlag(FileAttributes.Directory);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to check if file path is a directory.");
                return null;
            }
        }

        public static byte[] GetBufferSize(FileStream fileStream)
        {
            NullChecks.FileStreams(fileStream);
            int bufferSize = 4096;
            // Use a larger buffer for bigger files
            if (fileStream.Length >= Constants.Mebibyte)
            {
                // 128 KiB
                bufferSize = 131072;
            }
            else if (bufferSize > fileStream.Length)
            {
                // Use file size as buffer for small files
                return new byte[fileStream.Length];
            }
            return new byte[bufferSize];
        }

        public static void OverwriteFile(string fileToDelete, string fileToCopy)
        {
            try
            {
                File.Copy(fileToCopy, fileToDelete, true);
                File.Delete(fileToDelete);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorResultsText(fileToDelete, ex.GetType().Name, "Unable to overwrite and/or delete.");
            }
        }

        public static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to delete the file.");
            }
        }

        public static void MakeFileReadOnly(string filePath)
        {
            try
            {
                File.SetAttributes(filePath, FileAttributes.ReadOnly);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to make the file read-only.");
            }
        }
    }
}
