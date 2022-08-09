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
using System.IO.Compression;
using System.Linq;
using Geralt;

namespace Kryptor;

public static class FileHandling
{
    private static readonly char[] SeparatorChars = {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar};
    
    public static bool IsDirectory(string filePath) => File.GetAttributes(filePath).HasFlag(FileAttributes.Directory);
    
    public static bool IsDirectoryEmpty(string directoryPath) => !Directory.EnumerateFiles(directoryPath, searchPattern: "*", SearchOption.AllDirectories).Any();
    
    public static string[] GetAllFiles(string directoryPath) => Directory.GetFiles(directoryPath, searchPattern: "*", SearchOption.AllDirectories);
    
    public static long GetFileLength(string filePath) => new FileInfo(filePath).Length;
    
    public static bool HasKryptorExtension(string filePath) => filePath.EndsWith(Constants.EncryptedExtension, StringComparison.Ordinal);
    
    public static string TrimTrailingSeparatorChars(string filePath) => filePath.TrimEnd(SeparatorChars);
    
    public static string ReplaceFileName(string originalFilePath, string newFileName)
    {
        string directoryPath = Path.GetDirectoryName(Path.GetFullPath(originalFilePath));
        string newPath = Path.GetFullPath(Path.Combine(directoryPath, newFileName));
        if (!newPath.StartsWith(Path.GetFullPath(directoryPath))) { throw new ArgumentException("Invalid new path."); }
        return newPath;
    }
    
    public static string GetEncryptedOutputFilePath(string inputFilePath)
    {
        if (Globals.EncryptFileNames) { inputFilePath = ReplaceFileName(inputFilePath, SecureRandom.GetString(Constants.RandomFileNameLength)); }
        return GetUniqueFilePath(inputFilePath + Constants.EncryptedExtension);
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
            var magicBytes = ReadFileHeader(filePath, offset: 0, Constants.EncryptionMagicBytes.Length);
            return ConstantTime.Equals(magicBytes, Constants.EncryptionMagicBytes);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex)) 
        { 
            return null;
        }
    }
    
    public static bool? IsValidEncryptedFileVersion(string filePath)
    {
        try
        {
            var formatVersion = ReadFileHeader(filePath, Constants.EncryptionMagicBytes.Length, Constants.EncryptionVersion.Length);
            return ConstantTime.Equals(formatVersion, Constants.EncryptionVersion);
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
            return ConstantTime.Equals(magicBytes, Constants.SignatureMagicBytes);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            return null;
        }
    }
    
    public static bool? IsValidSignatureFileVersion(string filePath)
    {
        try
        {
            var formatVersion = ReadFileHeader(filePath, Constants.SignatureMagicBytes.Length, Constants.SignatureVersion.Length);
            return ConstantTime.Equals(formatVersion, Constants.SignatureVersion);
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

    public static void DeleteDirectory(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath)) { return; }
            foreach (string filePath in GetAllFiles(directoryPath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
            Directory.Delete(directoryPath, recursive: true);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to delete the directory.");
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
    
    public static string RenameFile(string filePath, string newFileName)
    {
        try
        {
            if (string.Equals(newFileName, RemoveFileNameNumber(Path.GetFileName(filePath)))) { return filePath; }
            string newFilePath = ReplaceFileName(filePath, newFileName);
            newFilePath = GetUniqueFilePath(newFilePath);
            Console.WriteLine($"Renaming \"{Path.GetFileName(filePath)}\" => \"{Path.GetFileName(newFilePath)}\"...");
            File.Move(filePath, newFilePath);
            return newFilePath;
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(filePath, ex.GetType().Name, "Unable to restore the original file name.");
            return filePath;
        }
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
    
    public static void CreateZipFile(string directoryPath, string zipFilePath)
    {
        DisplayMessage.CreatingZipFile(directoryPath, zipFilePath);
        ZipFile.CreateFromDirectory(directoryPath, zipFilePath, CompressionLevel.NoCompression, includeBaseDirectory: false);
        if (Globals.Overwrite) { DeleteDirectory(directoryPath); }
    }

    public static void ExtractZipFile(string zipFilePath)
    {
        try
        {
            string directoryPath = GetUniqueDirectoryPath(zipFilePath[..^Path.GetExtension(zipFilePath).Length]);
            DisplayMessage.ExtractingZipFile(zipFilePath, directoryPath);
            ZipFile.ExtractToDirectory(zipFilePath, directoryPath);
            DeleteFile(zipFilePath);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(zipFilePath, ex.GetType().Name, "Unable to extract the file.");
        }
    }
}