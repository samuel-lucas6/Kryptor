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

namespace KryptorCLI
{
    public static class ShredFiles
    {
        public static void ShredSelectedFiles(string[] filePaths)
        {
            NullChecks.StringArray(filePaths);
            Globals.SuccessfulCount = 0;
            Globals.TotalCount = filePaths.Length;
            // Use XChaCha20 - store current setting
            int selectedCipher = Globals.EncryptionAlgorithm;
            Globals.EncryptionAlgorithm = (int)Cipher.XChaCha20;
            foreach (string filePath in filePaths)
            {
                bool? directory = FileHandling.IsDirectory(filePath);
                if (directory != null)
                {
                    if (directory == false)
                    {
                        CallShredFilesMethod(filePath);
                    }
                    else
                    {
                        ShredDirectory(filePath);
                    }
                }
            }
            // Restore encryption algorithm setting
            Globals.EncryptionAlgorithm = selectedCipher;
        }

        private static void ShredDirectory(string directoryPath)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(directoryPath)
                {
                    Attributes = FileAttributes.NotContentIndexed
                };
                string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                Globals.TotalCount += files.Length;
                foreach (string filePath in files)
                {
                    CallShredFilesMethod(filePath);
                }
                string anonymisedDirectoryPath = AnonymousRename.MoveFile(directoryPath, false);
                Directory.Delete(anonymisedDirectoryPath, true);
                Console.WriteLine($"{Path.GetFileName(directoryPath)}: Folder erasure successful.{Environment.NewLine}");
                Globals.SuccessfulCount += 1;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.Error(directoryPath, ex.GetType().Name, "Unable to access the directory.");
            }
        }

        private static void CallShredFilesMethod(string filePath)
        {
            try
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                switch (Globals.ShredFilesMethod)
                {
                    case 0:
                        ShredFilesMethods.FirstLast16KiB(filePath);
                        break;
                    case 1:
                        // false = use zeros, not ones (used in Infosec Standard 5 Enhanced)
                        ShredFilesMethods.ZeroFill(filePath, false);
                        break;
                    case 2:
                        ShredFilesMethods.PseudorandomData(filePath);
                        break;
                    case 3:
                        ShredFilesMethods.EncryptionErasure(filePath);
                        break;
                    case 4:
                        ShredFilesMethods.InfosecStandard5Enhanced(filePath);
                        break;
                    case 5:
                        ShredFilesMethods.PseudorandomData5Passes(filePath);
                        break;
                }
                DeleteFile(filePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.Error(filePath, ex.GetType().Name, "Unable to set file attributes to normal. This file has not been shredded.");
            }
        }

        private static void DeleteFile(string filePath)
        {
            try
            {
                string anonymisedFilePath = AnonymousRename.MoveFile(filePath, true);
                EraseFileMetadata(anonymisedFilePath);
                File.Delete(anonymisedFilePath);
                Console.WriteLine($"{Path.GetFileName(filePath)}: File erasure successful.");
                Globals.SuccessfulCount += 1;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.Error(filePath, ex.GetType().Name, "Unable to delete the file.");
            }
        }

        public static void EraseFileMetadata(string filePath)
        {
            try
            {
                var eraseDate = new DateTime(2001, 11, 26, 12, 0, 0);
                File.SetCreationTime(filePath, eraseDate);
                File.SetLastWriteTime(filePath, eraseDate);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.Error(filePath, ex.GetType().Name, "Erasure of file metadata failed.");
            }
        }
    }
}
