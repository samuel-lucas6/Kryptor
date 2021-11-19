using System;
using System.IO;

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
    public static class RestoreDirectoryNames
    {
        public static void AllDirectories(string directoryPath)
        {
            string[] subdirectories = FileHandling.GetAllDirectories(directoryPath);
            for (int i = subdirectories.Length - 1; i >= 0; i--)
            {
                RestoreDirectoryName(subdirectories[i]);
            }
            RestoreDirectoryName(directoryPath);
        }

        private static void RestoreDirectoryName(string obfuscatedDirectoryPath)
        {
            try
            {
                string obfuscatedDirectoryName = Path.GetFileName(obfuscatedDirectoryPath);
                string storageFileName = $"{obfuscatedDirectoryName}.txt";
                string storageFilePath = Path.Combine(obfuscatedDirectoryPath, storageFileName);
                if (!File.Exists(storageFilePath)) { return; }
                string directoryName = File.ReadAllText(storageFilePath);
                string directoryPath = obfuscatedDirectoryPath.Replace(obfuscatedDirectoryName, directoryName);
                directoryPath = FileHandling.GetUniqueDirectoryPath(directoryPath);
                Console.WriteLine($"Renaming {obfuscatedDirectoryName} directory => {Path.GetFileName(directoryPath)}...");
                Directory.Move(obfuscatedDirectoryPath, directoryPath);
                storageFilePath = Path.Combine(directoryPath, storageFileName);
                FileHandling.DeleteFile(storageFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                DisplayMessage.FilePathException(obfuscatedDirectoryPath, ex.GetType().Name, "Unable to restore the directory name.");
            }
        }
    }
}
