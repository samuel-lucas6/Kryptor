using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using Sodium;

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

namespace KryptorGUI
{
    public static class ShredFilesMethods
    {
        public static void FirstLast16KiB(string filePath)
        {
            try
            {
                const int sixteenKiB = 16384;
                byte[] first16KiB = SodiumCore.GetRandomBytes(sixteenKiB);
                byte[] last16KiB = SodiumCore.GetRandomBytes(sixteenKiB);
                using (var firstFileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.RandomAccess))
                {
                    firstFileStream.Write(first16KiB, 0, first16KiB.Length);
                    // Remove the last 16 KiB
                    firstFileStream.SetLength(firstFileStream.Length - last16KiB.Length);
                }
                using (var lastFileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, Constants.FileBufferSize, FileOptions.RandomAccess))
                {
                    lastFileStream.Write(last16KiB, 0, last16KiB.Length);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "'First/Last 16KiB' erasure failed.");
            }
        }

        public static void PseudorandomData(string filePath, BackgroundWorker bgwShredFiles)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                {
                    byte[] randomBytes = FileHandling.GetBufferSize(fileStream);
                    while (fileStream.Position < fileStream.Length)
                    {
                        randomBytes = SodiumCore.GetRandomBytes(randomBytes.Length);
                        fileStream.Write(randomBytes, 0, randomBytes.Length);
                        ReportProgress.ReportEncryptionProgress(fileStream.Position, fileStream.Length, bgwShredFiles);
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "'1 Pass' erasure failed.");
            }
        }

        public static void ZeroFill(string filePath, bool useOnes, BackgroundWorker bgwShredFiles)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                {
                    byte[] zeroes = FileHandling.GetBufferSize(fileStream);
                    if (useOnes == true)
                    {
                        zeroes = FillArrayWithOnes(zeroes);
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
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "'Zero fill' erasure failed.");
            }
        }

        private static byte[] FillArrayWithOnes(byte[] zeroes)
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
            // First pass with zeros
            ZeroFill(filePath, false, bgwShredFiles);
            // Second pass with ones
            ZeroFill(filePath, true, bgwShredFiles);
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
                using (var ciphertext = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                using (var plaintext = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                {
                    byte[] fileBytes = FileHandling.GetBufferSize(plaintext);
                    byte[] key = SodiumCore.GetRandomBytes(Constants.EncryptionKeySize);
                    byte[] nonce = SodiumCore.GetRandomBytes(Constants.XChaChaNonceLength);
                    StreamCiphers.Encrypt(plaintext, ciphertext, 0, fileBytes, nonce, key, bgwShredFiles);
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
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "'Encryption' erasure failed.");
            }
        }
    }
}
