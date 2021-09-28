using System;
using System.IO;
using System.Text;

/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2021 Samuel Lucas

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
    public static class RestoreFileName
    {
        public static void RenameFile(string outputFilePath, int fileNameLength)
        {
            try
            {
                if (fileNameLength == 0) { return; }
                string originalFileName = ReadFileName(outputFilePath, fileNameLength);
                string obfuscatedFileName = Path.GetFileName(outputFilePath);
                if (string.Equals(originalFileName, FileHandling.RemoveFileNameNumber(obfuscatedFileName))) { return; }
                string restoredFilePath = outputFilePath.Replace(obfuscatedFileName, originalFileName);
                restoredFilePath = FileHandling.GetUniqueFilePath(restoredFilePath);
                DisplayMessage.MessageNewLine($"Renaming {Path.GetFileName(outputFilePath)} => {Path.GetFileName(restoredFilePath)}...");
                File.Move(outputFilePath, restoredFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                DisplayMessage.FilePathException(outputFilePath, ex.GetType().Name, "Unable to restore the original file name.");
            }
        }

        private static string ReadFileName(string outputFilePath, int fileNameLength)
        {
            byte[] fileName = new byte[fileNameLength];
            using var fileStream = new FileStream(outputFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
            fileStream.Seek(-fileNameLength, SeekOrigin.End);
            fileStream.Read(fileName, offset: 0, fileName.Length);
            fileStream.SetLength(fileStream.Length - fileNameLength);
            return Encoding.UTF8.GetString(fileName);
        }

        public static void RemoveAppendedFileName(string inputFilePath)
        {
            try
            {
                File.SetAttributes(inputFilePath, FileAttributes.Normal);
                string fileName = Path.GetFileName(inputFilePath);
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
                inputFile.SetLength(inputFile.Length - fileNameBytes.Length);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to remove appended file name.");
            }
        }
    }
}
