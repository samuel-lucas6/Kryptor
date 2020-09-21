using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;

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
    public static class ShredFilesMethods
    {
        public static void FirstLast16KiB(string filePath)
        {
            try
            {
                const int sixteenKiB = 16384;
                byte[] first16KiB = RandomNumberGenerator.GenerateRandomBytes(sixteenKiB);
                byte[] last16KiB = RandomNumberGenerator.GenerateRandomBytes(sixteenKiB);
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    fileStream.Write(first16KiB, 0, first16KiB.Length);
                    // Remove the last 16 KiB
                    fileStream.SetLength(fileStream.Length - last16KiB.Length);
                }
                using (var fsAppend = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    fsAppend.Write(last16KiB, 0, last16KiB.Length);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "First/Last 16KiB erasure failed.");
            }
        }

        public static void PseudorandomData(string filePath, BackgroundWorker bgwShredFiles)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    while (fileStream.Position < fileStream.Length)
                    {
                        byte[] randomBytes = RandomNumberGenerator.GenerateRandomBytes(4096);
                        fileStream.Write(randomBytes, 0, randomBytes.Length);
                        ReportProgress.ReportEncryptionProgress(fileStream.Position, fileStream.Length, bgwShredFiles);
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Pseudorandom data erasure failed.");
            }
        }

        public static void ZeroFill(string filePath, bool useOnes, BackgroundWorker bgwShredFiles)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    byte[] zeroes = new byte[4096];
                    if (useOnes == true)
                    {
                        zeroes = ConvertToOnes(zeroes);
                    }
                    while (fileStream.Position < fileStream.Length)
                    {
                        fileStream.Write(zeroes, 0, zeroes.Length);
                        ReportProgress.ReportEncryptionProgress(fileStream.Position, fileStream.Length, bgwShredFiles);
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Zero fill erasure failed.");
            }
        }

        private static byte[] ConvertToOnes(byte[] zeroes)
        {
            // Convert array of zeros to an array of ones
            byte one = Convert.ToByte(1);
            for (int i = 0; i < zeroes.Length; i++)
            {
                zeroes[i] = one;
            }
            return zeroes;
        }

        public static void InfosecStandard5Enhanced(string filePath, BackgroundWorker bgwShredFiles)
        {
            // First pass with ones
            ZeroFill(filePath, true, bgwShredFiles);
            // Second pass with zeros
            ZeroFill(filePath, false, bgwShredFiles);
            // Third pass with random data
            PseudorandomData(filePath, bgwShredFiles);
        }

        public static void PseudorandomData5Passes(string filePath, BackgroundWorker bgwShredFiles)
        {
            for (int i = 0; i < 5; i++)
            {
                PseudorandomData(filePath, bgwShredFiles);
            }
        }

        public static void EncryptionErasure(string filePath, BackgroundWorker bgwShredFiles)
        {
            try
            {
                string encryptedFilePath = AnonymousRename.GetAnonymousFileName(filePath);
                using (var ciphertext = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                using (var plaintext = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    byte[] fileBytes = new byte[4096];
                    byte[] key = RandomNumberGenerator.GenerateRandomBytes(Constants.EncryptionKeySize);
                    byte[] nonce = RandomNumberGenerator.GenerateRandomBytes(16);
                    AesAlgorithms.EncryptAesCBC(plaintext, ciphertext, fileBytes, nonce, key, bgwShredFiles);
                    Utilities.ZeroArray(key);
                    Utilities.ZeroArray(nonce);
                }
                // Overwrite the original file
                File.Copy(encryptedFilePath, filePath, true);
                ShredFiles.EraseFileMetadata(encryptedFilePath);
                File.Delete(encryptedFilePath);
            }
            catch (Exception ex) when (ex is CryptographicException || ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Encryption erasure failed.");
            }
        }
    }
}
