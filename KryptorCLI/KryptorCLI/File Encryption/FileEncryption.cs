using System;
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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace KryptorCLI
{
    public static class FileEncryption
    {
        public static byte[] GetPasswordBytes(char[] password)
        {
            byte[] passwordBytes = null;
            if (password != null && password.Length > 0)
            {
                passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = HashPasswordBytes(passwordBytes);
            }
            return passwordBytes;
        }

        private static byte[] HashPasswordBytes(byte[] passwordBytes)
        {
            passwordBytes = HashingAlgorithms.Blake2(passwordBytes);
            MemoryEncryption.EncryptByteArray(ref passwordBytes);
            return passwordBytes;
        }

        public static void StartEncryption(bool encryption, string[] filePaths, byte[] passwordBytes, string keyfilePath)
        {
            // Don't use keyfile bytes when only a keyfile is selected
            if (passwordBytes != null)
            {
                passwordBytes = GetKeyfileBytes(passwordBytes, keyfilePath);
            }
            else
            {
                passwordBytes = KeyfileAsPassword(keyfilePath);
            }
            GetFilePaths(encryption, filePaths, passwordBytes);
            Utilities.ZeroArray(passwordBytes);
        }

        private static byte[] GetKeyfileBytes(byte[] passwordBytes, string keyfilePath)
        {
            if (!string.IsNullOrEmpty(keyfilePath))
            {
                byte[] keyfileBytes = Keyfiles.ReadKeyfile(keyfilePath);
                if (keyfileBytes != null)
                {
                    MemoryEncryption.DecryptByteArray(ref passwordBytes);
                    // Combine password and keyfile bytes
                    passwordBytes = HashingAlgorithms.Blake2(passwordBytes, keyfileBytes);
                    MemoryEncryption.EncryptByteArray(ref passwordBytes);
                    Utilities.ZeroArray(keyfileBytes);
                }
            }
            return passwordBytes;
        }

        private static byte[] KeyfileAsPassword(string keyfilePath)
        {
            // If only a keyfile was selected, use the keyfile bytes as the password
            byte[] passwordBytes = Keyfiles.ReadKeyfile(keyfilePath);
            return HashPasswordBytes(passwordBytes);
        }

        public static void GetFilePaths(bool encryption, string[] filePaths, byte[] passwordBytes)
        {
            NullChecks.StringArray(filePaths);
            Globals.SuccessfulCount = 0;
            Globals.TotalCount = filePaths.Length;
            foreach (string filePath in filePaths)
            {
                bool? fileIsDirectory = FileHandling.IsDirectory(filePath);
                if (fileIsDirectory != null)
                {
                    if (fileIsDirectory == false)
                    {
                        CallEncryption(encryption, filePath, passwordBytes);
                    }
                    else
                    {
                        DirectoryEncryption(encryption, filePath, passwordBytes);
                    }
                }
            }
        }

        private static void DirectoryEncryption(bool encryption, string folderPath, byte[] passwordBytes)
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
                    CallEncryption(encryption, filePath, passwordBytes);
                }
                // Deanonymise directory names after decryption (if enabled)
                AnonymousRename.DeanonymiseDirectories(encryption, folderPath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.Error(folderPath, ex.GetType().Name, "Unable to get files in the directory.");
            }
        }

        private static void CallEncryption(bool encryption, string filePath, byte[] passwordBytes)
        {
            try
            {
                bool kryptorExtension = filePath.EndsWith(Constants.EncryptedExtension, StringComparison.Ordinal);
                // Prevent Read-Only file attribute causing errors
                File.SetAttributes(filePath, FileAttributes.Normal);
                if (encryption == true && kryptorExtension == false)
                {
                    Encryption.InitializeEncryption(filePath, passwordBytes);
                    OverwriteDisabled(filePath);
                }
                else if (encryption == false && kryptorExtension == true)
                {
                    Decryption.InitializeDecryption(filePath, passwordBytes);
                }
                else
                {
                    DisplayMessage.FileError(filePath, encryption, kryptorExtension);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.Error(filePath, ex.GetType().Name, "Unable to set file attributes to normal.");
            }
        }

        private static void OverwriteDisabled(string filePath)
        {
            if (Globals.AnonymousRename == true && Globals.OverwriteFiles == false)
            {
                // Remove appended file name from the original file
                OriginalFileName.ReadOriginalFileName(filePath);
            }
        }
    }
}
