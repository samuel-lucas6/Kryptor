/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2022 Samuel Lucas

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

using System;
using System.IO;
using System.Text;

namespace KryptorCLI;

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
            string restoredFilePath = FileHandling.ReplaceFileName(outputFilePath, originalFileName);
            restoredFilePath = FileHandling.GetUniqueFilePath(restoredFilePath);
            Console.WriteLine($"Renaming \"{Path.GetFileName(outputFilePath)}\" => \"{Path.GetFileName(restoredFilePath)}\"...");
            File.Move(outputFilePath, restoredFilePath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(outputFilePath, ex.GetType().Name, "Unable to restore the original file name.");
        }
    }

    private static string ReadFileName(string outputFilePath, int fileNameLength)
    {
        using var fileStream = new FileStream(outputFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
        fileStream.Seek(offset: -fileNameLength, SeekOrigin.End);
        var fileName = new byte[fileNameLength];
        fileStream.Read(fileName, offset: 0, fileName.Length);
        fileStream.SetLength(fileStream.Length - fileNameLength);
        return Encoding.UTF8.GetString(fileName);
    }

    public static void RemoveAppendedFileName(string inputFilePath)
    {
        try
        {
            File.SetAttributes(inputFilePath, FileAttributes.Normal);
            using var inputFile = new FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
            var fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(inputFilePath));
            inputFile.SetLength(inputFile.Length - fileNameBytes.Length);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to remove the appended file name.");
        }
    }
}