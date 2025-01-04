/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2025 Samuel Lucas

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
using System.Linq;
using System.IO.Compression;

namespace Kryptor;

public static class FileHandling
{
    public static bool? IsDirectoryEmpty(string directoryPath)
    {
        try
        {
            return !Directory.EnumerateFiles(directoryPath, searchPattern: "*", SearchOption.AllDirectories).Any();
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            return null;
        }
    }

    public static string TrimTrailingSeparatorChars(string filePath) => filePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar);

    public static string ReplaceFileName(string filePath, string newFileName)
    {
        string directoryPath = Path.GetDirectoryName(Path.GetFullPath(filePath));
        string newPath = Path.GetFullPath(Path.Combine(directoryPath, newFileName));
        if (!newPath.StartsWith(Path.GetFullPath(directoryPath + Path.DirectorySeparatorChar))) {
            throw new ArgumentException("Invalid new path.");
        }
        return newPath;
    }

    public static FileStreamOptions GetFileStreamReadOptions(string filePath)
    {
        long fileLength = new FileInfo(filePath).Length;
        return new FileStreamOptions
        {
            Mode = FileMode.Open,
            Access = FileAccess.Read,
            Share = FileShare.Read,
            BufferSize = GetFileStreamBufferSize(fileLength),
            Options = fileLength < 10737418240 ? FileOptions.None : FileOptions.SequentialScan
        };
    }

    private static int GetFileStreamBufferSize(long fileLength)
    {
        return fileLength switch
        {
            <= 262144 => 0,
            <= 5242880 => 81920,
            < 104857600 => 131072,
            >= 104857600 => 1048576
        };
    }

    public static FileStreamOptions GetFileStreamWriteOptions(long preAllocationSize)
    {
        return new FileStreamOptions
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
            Share = FileShare.Read,
            BufferSize = GetFileStreamBufferSize(preAllocationSize),
            Options = FileOptions.None,
            PreallocationSize = preAllocationSize
        };
    }

    public static void OverwriteFile(string fileToDelete, string fileToCopy)
    {
        try
        {
            File.SetAttributes(fileToDelete, FileAttributes.Normal);
            File.Copy(fileToCopy, fileToDelete, overwrite: true);
            File.Delete(fileToDelete);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(fileToDelete, ex.GetType().Name, "Unable to overwrite the file.");
        }
    }

    public static void DeleteFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) {
                return;
            }
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(filePath, ex.GetType().Name, "Unable to delete the file.");
        }
    }

    public static string GetUniqueFilePath(string filePath)
    {
        if (!File.Exists(filePath)) {
            return filePath;
        }
        string fileNameNoExtension = Path.GetFileNameWithoutExtension(filePath);
        int fileNumber = 2;
        string fileExtension = Path.GetExtension(filePath);
        do {
            string newFileName = $"{fileNameNoExtension} ({fileNumber}){fileExtension}";
            filePath = ReplaceFileName(filePath, newFileName);
            fileNumber++;
        } while (File.Exists(filePath));
        return filePath;
    }

    public static string RemoveFileNameNumber(string filePath)
    {
        if (!filePath.EndsWith(')') || !char.IsDigit(filePath[^2])) {
            return filePath;
        }
        int index = filePath.LastIndexOf(" (", StringComparison.Ordinal);
        return filePath[..index];
    }

    public static string RenameFile(string filePath, string newFileName)
    {
        try
        {
            if (string.Equals(newFileName, RemoveFileNameNumber(Path.GetFileName(filePath)))) {
                return filePath;
            }
            string newFilePath = ReplaceFileName(filePath, newFileName);
            newFilePath = GetUniqueFilePath(newFilePath);
            DisplayMessage.InputToOutput("Renaming", filePath, newFilePath);
            File.Move(filePath, newFilePath);
            return newFilePath;
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(filePath, ex.GetType().Name, $"Unable to restore the file name. You should manually rename it to \"{newFileName}\".");
            return filePath;
        }
    }

    public static void CreateZipFile(string directoryPath, string zipFilePath)
    {
        DisplayMessage.InputToOutput("Zipping", directoryPath, zipFilePath);
        ZipFile.CreateFromDirectory(directoryPath, zipFilePath, CompressionLevel.NoCompression, includeBaseDirectory: false);
        if (Globals.Overwrite) {
            DeleteDirectory(directoryPath);
        }
    }

    private static void DeleteDirectory(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath)) {
                return;
            }
            foreach (string filePath in Directory.GetFiles(directoryPath, searchPattern: "*", SearchOption.AllDirectories)) {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
            Directory.Delete(directoryPath, recursive: true);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to delete the directory.");
        }
    }

    public static void ExtractZipFile(string zipFilePath)
    {
        try
        {
            string directoryPath = GetUniqueDirectoryPath(Path.ChangeExtension(zipFilePath, extension: null));
            DisplayMessage.InputToOutput("Extracting", zipFilePath, directoryPath);
            ZipFile.ExtractToDirectory(zipFilePath, directoryPath);
            DeleteFile(zipFilePath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(zipFilePath, ex.GetType().Name, "Unable to extract the file.");
        }
    }

    private static string GetUniqueDirectoryPath(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) {
            return directoryPath;
        }
        directoryPath = TrimTrailingSeparatorChars(directoryPath);
        string directoryName = Path.GetFileName(directoryPath);
        int directoryNumber = 2;
        do {
            directoryPath = ReplaceFileName(directoryPath, $"{directoryName} ({directoryNumber})");
            directoryNumber++;
        } while (Directory.Exists(directoryPath));
        return directoryPath;
    }
}
