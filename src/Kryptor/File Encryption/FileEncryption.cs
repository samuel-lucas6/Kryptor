using System;
using System.ComponentModel;
using System.IO;
using System.Text;

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
    public static class FileEncryption
    {
        public static byte[] GetPasswordBytes(char[] password)
        {
            if (password != null)
            {
                byte[] passwordBytes;
                if (password.Length > 0)
                {
                    passwordBytes = Encoding.UTF8.GetBytes(password);
                    passwordBytes = HashingAlgorithms.Blake2(passwordBytes);
                }
                else
                {
                    // If only a keyfile was selected, use the keyfile bytes as the password
                    passwordBytes = Keyfiles.ReadKeyfile(Globals.KeyfilePath);
                }
                MemoryEncryption.EncryptByteArray(ref passwordBytes);
                return passwordBytes;
            }
            else
            {
                return null;
            }
        }

        public static byte[] GetKeyfileBytes(ref byte[] passwordBytes)
        {
            byte[] keyfileBytes;
            if (!string.IsNullOrEmpty(Globals.KeyfilePath))
            {
                keyfileBytes = Keyfiles.ReadKeyfile(Globals.KeyfilePath);
                if (keyfileBytes != null)
                {
                    MemoryEncryption.DecryptByteArray(ref passwordBytes);
                    // Mix password and keyfile bytes
                    passwordBytes = HashingAlgorithms.HMAC(passwordBytes, keyfileBytes);
                    MemoryEncryption.EncryptByteArray(ref passwordBytes);
                    MemoryEncryption.EncryptByteArray(ref keyfileBytes);
                }
                return keyfileBytes;
            }
            else
            {
                // No keyfile selected
                return null;
            }
        }

        public static void GetFilePaths(bool encryption, byte[] passwordBytes, byte[] keyfileBytes, BackgroundWorker backgroundWorker)
        {
            int progress = 0;
            Globals.SuccessfulCount = 0;
            Globals.TotalCount = Globals.GetSelectedFiles().Count;
            byte[] associatedData = Generate.AssociatedData();
            foreach (string filePath in Globals.GetSelectedFiles())
            {
                bool? fileIsDirectory = CheckIsDirectory.PathIsDirectory(filePath);
                if (fileIsDirectory != null)
                {
                    if (fileIsDirectory == false)
                    {
                        CallEncryption(encryption, filePath, passwordBytes, keyfileBytes, associatedData, ref progress, backgroundWorker);
                    }
                    else
                    {
                        DirectoryEncryption(encryption, filePath, passwordBytes, keyfileBytes, associatedData, ref progress, backgroundWorker);
                    }
                }
            }
        }

        private static void DirectoryEncryption(bool encryption, string folderPath, byte[] passwordBytes, byte[] keyfileBytes, byte[] associatedData, ref int progress, BackgroundWorker backgroundWorker)
        {
            try
            {
                // Anonymise directory names before encryption (if enabled)
                folderPath = AnonymousRename.AnonymiseDirectories(encryption, folderPath);
                // Get all files in all directories
                string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
                // -1 for the selected directory
                Globals.TotalCount += files.Length - 1;
                foreach (string filePath in files)
                {
                    CallEncryption(encryption, filePath, passwordBytes, keyfileBytes, associatedData, ref progress, backgroundWorker);
                }
                // Deanonymise directory names after decryption (if enabled)
                AnonymousRename.DeanonymiseDirectories(encryption, folderPath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(folderPath, ex.GetType().Name, "Unable to retrieve file paths in the directory.");
            }
        }

        private static void CallEncryption(bool encryption, string filePath, byte[] passwordBytes, byte[] keyfileBytes, byte[] associatedData, ref int progress, BackgroundWorker backgroundWorker)
        {
            try
            {
                bool kryptorExtension = filePath.EndsWith(Constants.EncryptedExtension, StringComparison.Ordinal);
                // Prevent Read-Only file attribute causing errors
                File.SetAttributes(filePath, FileAttributes.Normal);
                if (encryption == true & kryptorExtension == false)
                {
                    Encryption.InitializeEncryption(filePath, passwordBytes, keyfileBytes, associatedData, backgroundWorker);
                    IfOverwriteDisabled(filePath);
                }
                else if (encryption == false & kryptorExtension == true)
                {
                    Decryption.InitializeDecryption(filePath, passwordBytes, keyfileBytes, associatedData, backgroundWorker);
                }
                else
                {
                    DisplayFileError(filePath, encryption, kryptorExtension);
                }
                ReportProgress.IncrementProgress(ref progress, backgroundWorker);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to set file attributes to normal - encryption/decryption cancelled.");
            }
        }

        private static void IfOverwriteDisabled(string filePath)
        {
            if (Globals.AnonymousRename == true & Globals.OverwriteFiles == false)
            {
                // Remove appended file name from the original file
                OriginalFileName.ReadOriginalFileName(filePath);
            }
        }

        private static void DisplayFileError(string filePath, bool encryption, bool kryptorExtension)
        {
            string errorMessage;
            if (encryption == false & kryptorExtension == false)
            {
                errorMessage = "This file is missing the '.kryptor' extension.";
            }
            else
            {
                errorMessage = "This file is already encrypted.";
            }
            Globals.ResultsText += $"{Path.GetFileName(filePath)} - Error: {errorMessage}" + Environment.NewLine;
        }

        public static void EncryptionDecryptionFailed(byte[] encryptionKey, byte[] hmacKey, string filePath)
        {
            Utilities.ZeroArray(encryptionKey);
            Utilities.ZeroArray(hmacKey);
            try
            {
                if (File.Exists(filePath))
                {
                    // Delete the created file (which is empty due to an exception)
                    File.Delete(filePath);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to delete the empty file. This file can be manually deleted.");
            }
        }
    }
}
