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
    public static class ObfuscateDirectoryNames
    {
        public static string AllDirectories(string directoryPath)
        {
            string[] subdirectories = FileHandling.GetAllDirectories(directoryPath);
            // Obfuscate subdirectory names - bottom most first
            for (int i = subdirectories.Length - 1; i >= 0; i--)
            {
                ObfuscateDirectoryName(subdirectories[i]);
            }
            // Obfuscate parent directory name
            return ObfuscateDirectoryName(directoryPath);
        }

        private static string ObfuscateDirectoryName(string directoryPath)
        {
            try
            {
                string directoryName = Path.GetFileName(directoryPath);
                string obfuscatedPath = ObfuscateFileName.ReplaceFilePath(directoryPath);
                DisplayMessage.MessageNewLine($"Renaming {directoryName} directory => {Path.GetFileName(obfuscatedPath)}...");
                Directory.Move(directoryPath, obfuscatedPath);
                StoreDirectoryName(directoryName, obfuscatedPath);
                return obfuscatedPath;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                DisplayMessage.FilePathException(directoryPath, ex.GetType().Name, "Unable to obfuscate directory name.");
                return directoryPath;
            }
        }

        private static void StoreDirectoryName(string directoryName, string obfuscatedPath)
        {
            // Store the original directory name in a text file inside the directory
            string storageFilePath = Path.Combine(obfuscatedPath, $"{Path.GetFileName(obfuscatedPath)}.txt");
            File.WriteAllText(storageFilePath, directoryName);
        }
    }
}
