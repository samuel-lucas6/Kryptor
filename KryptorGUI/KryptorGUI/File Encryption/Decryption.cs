using Sodium;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

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

namespace KryptorGUI
{
    public static class Decryption
    {
        public static void InitializeDecryption(string filePath, byte[] passwordBytes, BackgroundWorker bgwDecryption)
        {
            (int memorySize, int iterations, int parametersLength) = ReadFileHeaders.ReadArgon2Parameters(filePath);
            if (memorySize != 0)
            {
                byte[] salt = ReadFileHeaders.ReadSalt(filePath, parametersLength);
                (byte[] encryptionKey, byte[] macKey) = KeyDerivation.DeriveKeys(passwordBytes, salt, iterations, memorySize);
                CheckForTampering(filePath, parametersLength, encryptionKey, macKey, bgwDecryption);
            }
            else
            {
                Globals.ResultsText += $"{Path.GetFileName(filePath)}: The Argon2 parameters could not be read from the file.{Environment.NewLine}";
            }
        }

        private static void CheckForTampering(string filePath, int parametersLength, byte[] encryptionKey, byte[] macKey, BackgroundWorker bgwDecryption)
        {
            bool fileTampered = FileAuthentication.AuthenticateFile(filePath, macKey, out byte[] macBackup);
            Utilities.ZeroArray(macKey);
            if (fileTampered == false)
            {
                DecryptFile(filePath, parametersLength, macBackup, encryptionKey, bgwDecryption);
            }
            else
            {
                Globals.ResultsText += $"{Path.GetFileName(filePath)}: Incorrect password/keyfile or this file has been tampered with.{Environment.NewLine}";
                Utilities.ZeroArray(encryptionKey);
            }
        }

        private static void DecryptFile(string filePath, int parametersLength, byte[] macBackup, byte[] encryptionKey, BackgroundWorker bgwDecryption)
        {
            try
            {
                string decryptedFilePath = Regex.Replace(filePath, Constants.EncryptedExtension, string.Empty);
                int headersLength = Constants.SaltLength + parametersLength;
                using (var plaintext = new FileStream(decryptedFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                using (var ciphertext = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                {
                    // Skip the header bytes
                    ciphertext.Seek(headersLength, SeekOrigin.Begin);
                    byte[] fileBytes = FileHandling.GetBufferSize(ciphertext.Length);
                    // Generate a counter starting at 0
                    byte[] counter = Generate.Counter();
                    int bytesRead;
                    MemoryEncryption.DecryptByteArray(ref encryptionKey);
                    while ((bytesRead = ciphertext.Read(fileBytes, 0, fileBytes.Length)) > 0)
                    {
                        byte[] decryptedBytes = StreamEncryption.DecryptXChaCha20(fileBytes, counter, encryptionKey);
                        plaintext.Write(decryptedBytes, 0, bytesRead);
                        counter = Sodium.Utilities.Increment(counter);
                        // Report progress if decrypting a single file
                        ReportProgress.ReportEncryptionProgress(plaintext.Position, ciphertext.Length, bgwDecryption);
                    }
                    Utilities.ZeroArray(encryptionKey);
                }
                CompleteDecryption(filePath, decryptedFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileEncryptionExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Failed to backup the MAC. This data is required for decryption.");
                Utilities.ZeroArray(encryptionKey);
                RestoreMAC(filePath, macBackup);
            }
        }

        private static void RestoreMAC(string filePath, byte[] macBackup)
        {
            bool restored = FileAuthentication.AppendHash(filePath, macBackup);
            if (restored == false)
            {
                try
                {
                    File.WriteAllBytes($"{filePath}.backup", macBackup);
                }
                catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
                {
                    Logging.LogException(ex.ToString(), Logging.Severity.High);
                    DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Failed to backup the MAC. This data is required for decryption.");
                }
            }
            Utilities.ZeroArray(macBackup);
        }

        private static void CompleteDecryption(string filePath, string decryptedFilePath)
        {
            // Deanonymise file name
            OriginalFileName.RestoreOriginalFileName(decryptedFilePath);
            FileHandling.DeleteFile(filePath);
            Globals.ResultsText += $"{Path.GetFileName(filePath)}: File decryption successful.{Environment.NewLine}";
            Globals.SuccessfulCount += 1;
        }
    }
}
