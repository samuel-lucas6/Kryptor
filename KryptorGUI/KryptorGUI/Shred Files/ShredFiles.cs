using System;
using System.ComponentModel;
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
    public static class ShredFiles
    {
        public static void ShredSelectedFiles(BackgroundWorker bgwShredFiles)
        {
            int progress = 0;
            Globals.SuccessfulCount = 0;
            Globals.TotalCount = Globals.GetSelectedFiles().Count;
            // Use XChaCha20 - store current setting
            int selectedCipher = Globals.EncryptionAlgorithm;
            Globals.EncryptionAlgorithm = (int)Cipher.XChaCha20;
            foreach (string filePath in Globals.GetSelectedFiles())
            {
                bool? directory = FileHandling.IsDirectory(filePath);
                if (directory != null)
                {
                    if (directory == false)
                    {
                        CallShredFilesMethod(filePath, ref progress, bgwShredFiles);
                    }
                    else
                    {
                        ShredDirectory(filePath, ref progress, bgwShredFiles);
                    }
                }
            }
            // Restore encryption algorithm setting
            Globals.EncryptionAlgorithm = selectedCipher;
        }

        private static void ShredDirectory(string directoryPath, ref int progress, BackgroundWorker bgwShredFiles)
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
                    CallShredFilesMethod(filePath, ref progress, bgwShredFiles);
                }
                string anonymisedDirectoryPath = AnonymousRename.MoveFile(directoryPath, false);
                Directory.Delete(anonymisedDirectoryPath, true);
                Globals.ResultsText += $"{Path.GetFileName(directoryPath)}: Folder erasure successful.{Environment.NewLine}";
                Globals.SuccessfulCount += 1;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(directoryPath, ex.GetType().Name, "Unable to access the directory.");
            }
        }

        private static void CallShredFilesMethod(string filePath, ref int progress, BackgroundWorker bgwShredFiles)
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
                        ShredFilesMethods.ZeroFill(filePath, false, bgwShredFiles);
                        break;
                    case 2:
                        ShredFilesMethods.PseudorandomData(filePath, bgwShredFiles);
                        break;
                    case 3:
                        ShredFilesMethods.EncryptionErasure(filePath, bgwShredFiles);
                        break;
                    case 4:
                        ShredFilesMethods.InfosecStandard5Enhanced(filePath, bgwShredFiles);
                        break;
                    case 5:
                        ShredFilesMethods.PseudorandomData5Passes(filePath, bgwShredFiles);
                        break;
                }
                DeleteFile(filePath);
                ReportProgress.IncrementProgress(ref progress, bgwShredFiles);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to set file attributes to normal. This file has not been shredded.");
            }
        }

        private static void DeleteFile(string filePath)
        {
            try
            {
                string anonymisedFilePath = AnonymousRename.MoveFile(filePath, true);
                EraseFileMetadata(anonymisedFilePath);
                File.Delete(anonymisedFilePath);
                Globals.ResultsText += $"{Path.GetFileName(filePath)}: File erasure successful." + Environment.NewLine;
                Globals.SuccessfulCount += 1;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to delete the file.");
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
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Erasure of file metadata failed.");
            }
        }
    }
}
