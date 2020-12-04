using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

namespace KryptorGUI
{
    public static class OriginalFileName
    {
        public static bool AppendOriginalFileName(string filePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                EncodeFileName(filePath, fileName, out byte[] newLineBytes, out byte[] fileNameBytes);
                using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, Constants.FileBufferSize, FileOptions.RandomAccess))
                {
                    fileStream.Write(newLineBytes, 0, newLineBytes.Length);
                    fileStream.Write(fileNameBytes, 0, fileNameBytes.Length);
                }
                return true;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex) || ex is EncoderFallbackException)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Could not store original file name.");
                return false;
            }
        }

        private static void EncodeFileName(string filePath, string fileName, out byte[] newLineBytes, out byte[] fileNameBytes)
        {
            using (var streamReader = new StreamReader(filePath, Encoding.UTF8, true))
            {
                streamReader.Peek();
                var fileEncoding = streamReader.CurrentEncoding;
                newLineBytes = fileEncoding.GetBytes(Environment.NewLine);
                fileNameBytes = fileEncoding.GetBytes(fileName);
            }
        }

        public static void RestoreOriginalFileName(string decryptedFilePath)
        {
            try
            {
                if (Globals.AnonymousRename == true)
                {
                    string originalFileName = ReadOriginalFileName(decryptedFilePath);
                    if (!string.IsNullOrEmpty(originalFileName))
                    {
                        string anonymousFileName = Path.GetFileName(decryptedFilePath);
                        string originalFilePath = Regex.Replace(decryptedFilePath, anonymousFileName, originalFileName);
                        if (File.Exists(originalFilePath))
                        {
                            // Replace the file
                            File.Delete(originalFilePath);
                        }
                        File.Move(decryptedFilePath, originalFilePath);
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(decryptedFilePath, ex.GetType().Name, "Unable to restore original file name.");
            }
        }

        public static string ReadOriginalFileName(string filePath)
        {
            try
            {
                // Read the last line of the decrypted file
                string originalFileName = File.ReadLines(filePath).Last().Trim('\0');
                RemoveStoredFileName(filePath, originalFileName);
                return originalFileName;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex) || ex is InvalidOperationException)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to read original file name.");
                return string.Empty;
            }
        }

        private static void RemoveStoredFileName(string filePath, string originalFileName)
        {
            try
            {
                int fileNameLength = GetFileNameLength(filePath, originalFileName);
                if (fileNameLength != 0)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.RandomAccess))
                    {
                        fileStream.SetLength(fileStream.Length - fileNameLength);
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to remove the original file name stored in the decrypted file. The file should be decrypted, but there is a leftover string at the end of the file.");
            }
        }

        private static int GetFileNameLength(string filePath, string originalFileName)
        {
            try
            {
                EncodeFileName(filePath, originalFileName, out byte[] newLineBytes, out byte[] fileNameBytes);
                return newLineBytes.Length + fileNameBytes.Length;
            }
            catch (Exception ex) when (ExceptionFilters.CharacterEncodingExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to remove the original file name stored in the file. The length of the stored file name could not be calculated.");
                return 0;
            }
        }

        public static void RestoreDirectoryName(string folderPath)
        {
            try
            {
                NullChecks.Strings(folderPath);
                string anonymisedDirectoryName = Path.GetFileName(folderPath);
                // Get the path where the original directory name is stored
                string storageFileName = $"{anonymisedDirectoryName}.txt";
                string storageFilePath = Path.Combine(folderPath, storageFileName);
                if (File.Exists(storageFilePath))
                {
                    string originalDirectoryName = File.ReadAllText(storageFilePath);
                    string originalDirectoryPath = folderPath.Replace(anonymisedDirectoryName, originalDirectoryName);
                    Directory.Move(folderPath, originalDirectoryPath);
                    storageFilePath = Path.Combine(originalDirectoryPath, storageFileName);
                    if (File.Exists(storageFilePath))
                    {
                        File.Delete(storageFilePath);
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(folderPath, ex.GetType().Name, "Unable to restore original directory name.");
            }
        }
    }
}
