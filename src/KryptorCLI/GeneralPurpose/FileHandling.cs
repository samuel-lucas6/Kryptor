/*
    Kryptor: A simple, modern, and secure encryption tool.
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
using System.Linq;
using System.Text;
using Sodium;

namespace KryptorCLI;

public static class FileHandling
{
    public static bool IsDirectory(string filePath) => File.GetAttributes(filePath).HasFlag(FileAttributes.Directory);

    public static bool IsDirectoryEmpty(string directoryPath) => !Directory.EnumerateFiles(directoryPath, searchPattern: "*", SearchOption.AllDirectories).Any();

    public static string[] GetAllDirectories(string directoryPath) => Directory.GetDirectories(directoryPath, searchPattern: "*", SearchOption.AllDirectories);

    public static string[] GetAllFiles(string directoryPath) => Directory.GetFiles(directoryPath, searchPattern: "*", SearchOption.AllDirectories);

    public static long GetFileLength(string filePath) => new FileInfo(filePath).Length;

    public static bool HasKryptorExtension(string filePath) => filePath.EndsWith(Constants.EncryptedExtension, StringComparison.Ordinal);
    
    public static string GetRandomFileName() => Utilities.BinaryToBase64(SodiumCore.GetRandomBytes(count: 16), Utilities.Base64Variant.UrlSafeNoPadding).TrimStart('-');

    public static string RemoveIllegalFileNameChars(string fileName) => string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
    
    public static string ReplaceFileName(string originalFilePath, string newFileName)
    {
        string directoryPath = Path.GetDirectoryName(Path.GetFullPath(originalFilePath));
        string newPath = Path.GetFullPath(Path.Combine(directoryPath, newFileName));
        if (!newPath.StartsWith(Path.GetFullPath(directoryPath))) { throw new ArgumentException("Invalid new path."); }
        return newPath;
    }
    
    public static string GetEncryptedOutputFilePath(string inputFilePath)
    {
        try
        {
            if (Globals.EncryptFileNames)
            {
                AppendFileName(inputFilePath);
                inputFilePath = ReplaceFileName(inputFilePath, GetRandomFileName());
            }
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex) || ex is EncoderFallbackException)
        {
            DisplayMessage.FilePathException(inputFilePath, ex.GetType().Name, "Unable to store the file name.");
        }
        return GetUniqueFilePath(inputFilePath + Constants.EncryptedExtension);
    }
    
    public static void AppendFileName(string filePath)
    {
        File.SetAttributes(filePath, FileAttributes.Normal);
        using var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
        var fileNameBytes = Encoding.UTF8.GetBytes(RemoveIllegalFileNameChars(Path.GetFileName(filePath)));
        fileStream.Write(fileNameBytes, offset: 0, fileNameBytes.Length);
    }
    
    public static string GetDecryptedOutputFilePath(string inputFilePath) => GetUniqueFilePath(Path.ChangeExtension(inputFilePath, extension: null));

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

    public static bool? IsKryptorFile(string filePath)
    {
        try
        {
            var magicBytes = ReadFileHeader(filePath, offset: 0, Constants.KryptorMagicBytes.Length);
            return Utilities.Compare(magicBytes, Constants.KryptorMagicBytes);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex)) 
        { 
            return null;
        }
    }
    
    public static bool? IsSignatureFile(string filePath)
    {
        try
        {
            var magicBytes = ReadFileHeader(filePath, offset: 0, Constants.SignatureMagicBytes.Length);
            return Utilities.Compare(magicBytes, Constants.SignatureMagicBytes);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            return null;
        }
    }
    
    private static byte[] ReadFileHeader(string filePath, long offset, int length)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.SequentialScan);
        return ReadFileHeader(fileStream, offset, length);
    }

    public static void CopyDirectory(string sourceDirectoryPath, string destinationDirectoryPath, bool copySubdirectories)
    {
        var directoryInfo = new DirectoryInfo(sourceDirectoryPath);
        if (!directoryInfo.Exists) { throw new DirectoryNotFoundException("The source directory does not exist or could not be found."); }
        DirectoryInfo[] directories = directoryInfo.GetDirectories();
        destinationDirectoryPath = GetUniqueDirectoryPath(destinationDirectoryPath);
        Directory.CreateDirectory(destinationDirectoryPath);
        FileInfo[] files = directoryInfo.GetFiles();
        foreach (FileInfo file in files)
        {
            string newFilePath = Path.Combine(destinationDirectoryPath, file.Name);
            file.CopyTo(newFilePath, overwrite: false);
        }
        if (!copySubdirectories) { return; }
        foreach (DirectoryInfo subdirectory in directories)
        {
            string newSubdirectoryPath = Path.Combine(destinationDirectoryPath, subdirectory.Name);
            CopyDirectory(subdirectory.FullName, newSubdirectoryPath, copySubdirectories);
        }
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
            if (!File.Exists(filePath)) { return; }
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
        filePath = RemoveFileNameNumber(filePath);
        if (!File.Exists(filePath)) { return filePath; }
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        int fileNumber = 2;
        string fileExtension = Path.GetExtension(filePath);
        string directoryPath = Path.GetDirectoryName(filePath);
        do
        {
            string newFileName = $"{fileNameWithoutExtension} ({fileNumber}){fileExtension}";
            filePath = Path.Combine(directoryPath, newFileName);
            fileNumber++;
        }
        while (File.Exists(filePath));
        return filePath;
    }

    public static string RemoveFileNameNumber(string filePath)
    {
        string fileExtension = Path.GetExtension(filePath);
        if (!string.IsNullOrEmpty(fileExtension) && filePath.EndsWith(')') && filePath[^4].Equals(' ') && filePath[^3].Equals('(') && char.IsDigit(filePath[^2]))
        {
            return filePath.Remove(startIndex: filePath.Length - 4);
        }
        int lengthMinusExtension = filePath.Length - fileExtension.Length;
        if (filePath[lengthMinusExtension - 1].Equals(')') && filePath[lengthMinusExtension - 4].Equals(' ') && filePath[lengthMinusExtension - 3].Equals('(') && char.IsDigit(filePath[lengthMinusExtension - 2]))
        {
            return filePath.Remove(startIndex: lengthMinusExtension - 4) + fileExtension;
        }
        return filePath;
    }

    public static string GetUniqueDirectoryPath(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) { return directoryPath; }
        string parentDirectory = Directory.GetParent(directoryPath)?.FullName;
        string directoryName = Path.GetFileName(directoryPath);
        int directoryNumber = 2;
        do
        {
            directoryPath = Path.Combine(parentDirectory ?? string.Empty, $"{directoryName} ({directoryNumber})");
            directoryNumber++;
        }
        while (Directory.Exists(directoryPath));
        return directoryPath;
    }

    public static void SetFileAttributesReadOnly(string filePath)
    {
        try
        {
            File.SetAttributes(filePath, FileAttributes.ReadOnly);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(filePath, ex.GetType().Name, "Unable to mark the file as read-only.");
        }
    }
}