using Sodium;
using System;
using System.IO;

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
    public static class FileHandling
    {
        public static byte[] ReadFileHeader(string filePath, long offset, int length)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
            return ReadFileHeader(fileStream, offset, length);
        }

        public static byte[] ReadFileHeader(FileStream fileStream, long offset, int length)
        {
            var header = new byte[length];
            fileStream.Seek(offset, SeekOrigin.Begin);
            fileStream.Read(header, offset: 0, header.Length);
            return header;
        }

        public static byte[] ReadFileHeader(FileStream fileStream, int length)
        {
            var header = new byte[length];
            fileStream.Read(header, offset: 0, header.Length);
            return header;
        }

        public static bool IsDirectory(string filePath)
        {
            var fileAttributes = File.GetAttributes(filePath);
            return fileAttributes.HasFlag(FileAttributes.Directory);
        }

        public static string[] GetAllDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories);
        }

        public static string[] GetAllFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        }

        public static bool? IsKryptorFile(string filePath)
        {
            try
            {
                byte[] magicBytes = FileHeaders.ReadMagicBytes(filePath);
                return Utilities.Compare(magicBytes, Constants.KryptorMagicBytes);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                return null;
            }
        }

        public static bool HasKryptorExtension(string filePath)
        {
            return filePath.EndsWith(Constants.EncryptedExtension, StringComparison.Ordinal);
        }

        public static bool? IsSignatureFile(string filePath)
        {
            try
            {
                byte[] magicBytes = ReadFileHeader(filePath, offset: 0, Constants.SignatureMagicBytes.Length);
                return Utilities.Compare(magicBytes, Constants.SignatureMagicBytes);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                return null;
            }
        }

        public static long GetFileLength(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        public static void CopyDirectory(string sourceDirectoryPath, string destinationDirectoryPath, bool copySubdirectories)
        {
            var directoryInfo = new DirectoryInfo(sourceDirectoryPath);
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirectoryPath}");
            }
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            destinationDirectoryPath = GetUniqueDirectoryPath(destinationDirectoryPath);
            Directory.CreateDirectory(destinationDirectoryPath);
            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                string newFilePath = Path.Combine(destinationDirectoryPath, file.Name);
                file.CopyTo(newFilePath, false);
            }
            if (copySubdirectories)
            {
                foreach (DirectoryInfo subdirectory in directories)
                {
                    string newSubdirectoryPath = Path.Combine(destinationDirectoryPath, subdirectory.Name);
                    CopyDirectory(subdirectory.FullName, newSubdirectoryPath, copySubdirectories);
                }
            }
        }

        public static void OverwriteFile(string fileToDelete, string fileToCopy)
        {
            try
            {
                File.Copy(fileToCopy, fileToDelete, overwrite: true);
                DeleteFile(fileToDelete);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Warning);
                DisplayMessage.FilePathException(fileToDelete, ex.GetType().Name, "Unable to overwrite the file.");
            }
        }

        public static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Warning);
                DisplayMessage.FilePathException(filePath, ex.GetType().Name, "Unable to delete the file.");
            }
        }

        public static string GetUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath)) { return filePath; }
            int fileNumber = 1;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);
            string directoryPath = Path.GetDirectoryName(filePath);
            do
            {
                fileNumber++;
                string newFileName = $"{fileNameWithoutExtension} ({fileNumber}){fileExtension}";
                filePath = Path.Combine(directoryPath, newFileName);
            }
            while (File.Exists(filePath));
            return filePath;
        }

        public static string GetUniqueDirectoryPath(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) { return directoryPath; }
            int directoryNumber = 1;
            string parentDirectory = Directory.GetParent(directoryPath).FullName;
            string directoryName = Path.GetFileName(directoryPath);
            do
            {
                directoryNumber++;
                directoryPath = Path.Combine(parentDirectory, $"{directoryName} ({directoryNumber})");
            }
            while (Directory.Exists(directoryPath));
            return directoryPath;
        }

        public static void SetFileAttributesNormal(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
        }

        public static void SetFileAttributesReadOnly(string filePath)
        {
            try
            {
                File.SetAttributes(filePath, FileAttributes.ReadOnly);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Warning);
                DisplayMessage.FilePathException(filePath, ex.GetType().Name, "Unable to make the file read-only.");
            }
        }
    }
}
