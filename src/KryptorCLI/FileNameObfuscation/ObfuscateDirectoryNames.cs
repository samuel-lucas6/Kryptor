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

namespace KryptorCLI;

public static class ObfuscateDirectoryNames
{
    public static string AllDirectories(string directoryPath)
    {
        string[] subdirectories = FileHandling.GetAllDirectories(directoryPath);
        for (int i = subdirectories.Length - 1; i >= 0; i--)
        {
            ObfuscateDirectoryName(subdirectories[i]);
        }
        string obfuscatedDirectoryPath = ObfuscateDirectoryName(directoryPath);
        Console.WriteLine();
        return obfuscatedDirectoryPath;
    }

    private static string ObfuscateDirectoryName(string directoryPath)
    {
        try
        {
            string directoryName = FileHandling.RemoveIllegalFileNameChars(Path.GetFileName(directoryPath));
            string obfuscatedPath = FileHandling.ReplaceFileName(directoryPath, FileHandling.GetRandomFileName());
            Console.WriteLine($"Renaming \"{directoryName}\" directory => \"{Path.GetFileName(obfuscatedPath)}\"...");
            Directory.Move(directoryPath, obfuscatedPath);
            StoreDirectoryName(directoryName, obfuscatedPath);
            return obfuscatedPath;
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to obfuscate the directory name.");
            return directoryPath;
        }
    }

    private static void StoreDirectoryName(string directoryName, string obfuscatedPath)
    {
        string storageFilePath = Path.Combine(obfuscatedPath, $"{Path.GetFileName(obfuscatedPath)}.txt");
        File.WriteAllText(storageFilePath, directoryName);
    }
}