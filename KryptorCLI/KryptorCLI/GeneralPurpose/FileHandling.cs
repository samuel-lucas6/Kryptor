using System;
using System.IO;
using System.Text;

/*
    Kryptor: Free and open source file encryption.
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
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan);
            return ReadFileHeader(fileStream, offset, length);
        }

        public static byte[] ReadFileHeader(FileStream fileStream, long offset, int length)
        {
            byte[] header = new byte[length];
            fileStream.Seek(offset, SeekOrigin.Begin);
            fileStream.Read(header, offset: 0, header.Length);
            return header;
        }

        public static bool IsDirectory(string filePath)
        {
            FileAttributes fileAttributes = File.GetAttributes(filePath);
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
                return Sodium.Utilities.Compare(magicBytes, FileHeaders.GetMagicBytes());
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
                return Sodium.Utilities.Compare(magicBytes, DigitalSignatures.GetSignatureMagicBytes());
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

        public static void MakeFileReadOnly(string filePath)
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

        public static Encoding GetFileEncoding(string filePath)
        {
            using var streamReader = new StreamReader(filePath, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            streamReader.Peek();
            return streamReader.CurrentEncoding;
        }
    }
}
