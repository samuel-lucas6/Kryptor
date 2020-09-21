using System;
using System.IO;

/*  
    Kryptor: Free and open source file encryption software.
    Copyright(C) 2020 Samuel Lucas

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace Kryptor
{
    public static class AnonymousRename
    {
        public static string AnonymiseDirectories(bool encryption, string folderPath)
        {
            if (encryption == true & Globals.AnonymousRename == true)
            {
                string[] subdirectories = GetAllDirectories(folderPath);
                if (subdirectories != null)
                {
                    // Anonymise subdirectories - bottom most first
                    for (int i = subdirectories.Length - 1; i >= 0; i--)
                    {
                        AnonymiseDirectoryName(subdirectories[i]);
                    }
                }
                // Anonymise selected directory
                folderPath = AnonymiseDirectoryName(folderPath);
            }
            return folderPath;
        }

        private static string[] GetAllDirectories(string folderPath)
        {
            try
            {
                return Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(folderPath, ex.GetType().Name, "Unable to get subdirectories in selected folder.");
                return null;
            }
        }

        private static string AnonymiseDirectoryName(string folderPath)
        {
            try
            {
                string originalDirectoryName = Path.GetFileName(folderPath);
                string anonymisedPath = GetAnonymousFileName(folderPath);
                Directory.Move(folderPath, anonymisedPath);
                // Store the original directory name in a text file inside the directory
                string storageFilePath = Path.Combine(anonymisedPath, Path.GetFileName(anonymisedPath) + ".txt");
                File.WriteAllText(storageFilePath, originalDirectoryName);
                return anonymisedPath;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(folderPath, ex.GetType().Name, "Unable to anonymise directory name.");
                return folderPath;
            }
        }

        public static void DeanonymiseDirectories(bool encryption, string folderPath)
        {
            if (encryption == false & Globals.AnonymousRename == true)
            {
                string[] subdirectories = GetAllDirectories(folderPath);
                if (subdirectories != null)
                {
                    for (int i = subdirectories.Length - 1; i >= 0; i--)
                    {
                        OriginalFileName.RestoreDirectoryName(subdirectories[i]);
                    }
                }
                OriginalFileName.RestoreDirectoryName(folderPath);
            }
        }

        public static string GetAnonymousFileName(string filePath)
        {
            try
            {
                NullChecks.Strings(filePath);
                string originalFileName = Path.GetFileName(filePath);
                string randomFileName = GenerateRandomFileName();
                string anonymousFilePath = filePath.Replace(originalFileName, randomFileName);
                return anonymousFilePath;
            }
            catch (ArgumentException ex)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Bug);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to get anonymous file name. This is a bug - please report it.");
                return filePath;
            }
        }

        public static string GenerateRandomFileName()
        {
            // Remove the generated extension
            string randomFileName = Path.GetRandomFileName().Replace(".", string.Empty);
            randomFileName += Path.GetRandomFileName().Replace(".", string.Empty);
            return randomFileName;
        }
    }
}
