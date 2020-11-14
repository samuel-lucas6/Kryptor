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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace Kryptor
{
    public static class Decryption
    {
        public static void InitializeDecryption(string filePath, byte[] passwordBytes, BackgroundWorker bgwDecryption)
        {
            int[] argon2Parameters = ReadFileHeaders.ReadArgon2Parameters(filePath);
            if (argon2Parameters != null)
            {
                int parametersLength = argon2Parameters[2];
                byte[] salt = ReadFileHeaders.ReadSalt(filePath, parametersLength);
                byte[] nonce = ReadFileHeaders.ReadNonce(filePath, salt, parametersLength);
                var keys = KeyDerivation.DeriveKeys(passwordBytes, salt, argon2Parameters[1], argon2Parameters[0]);
                CheckForTampering(filePath, parametersLength, nonce, keys, bgwDecryption);
            }
            else
            {
                Globals.ResultsText += $"{Path.GetFileName(filePath)}: The Argon2 parameters could not be read from the file.{Environment.NewLine}";
            }
        }

        private static void CheckForTampering(string filePath, int parametersLength, byte[] nonce, (byte[], byte[]) keys, BackgroundWorker bgwDecryption)
        {
            bool fileTampered = FileAuthentication.AuthenticateFile(filePath, keys.Item2, out byte[] macBackup);
            Utilities.ZeroArray(keys.Item2);
            if (fileTampered == false)
            {
                DecryptFile(filePath, parametersLength, macBackup, nonce, keys.Item1, bgwDecryption);
            }
            else
            {
                Globals.ResultsText += $"{Path.GetFileName(filePath)}: Incorrect password/keyfile, wrong encryption algorithm, or this file has been tampered with.{Environment.NewLine}";
                Utilities.ZeroArray(keys.Item1);
            }
        }

        private static void DecryptFile(string filePath, int parametersLength, byte[] macBackup, byte[] nonce, byte[] key, BackgroundWorker bgwDecryption)
        {
            try
            {
                string decryptedFilePath = Regex.Replace(filePath, Constants.EncryptedExtension, string.Empty);
                // Get length of headers bytes (parameters, salt, nonce)
                int headersLength = ReadFileHeaders.GetHeadersLength(nonce.Length, parametersLength);
                using (var plaintext = new FileStream(decryptedFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                using (var ciphertext = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                {
                    // Skip the header bytes
                    ciphertext.Seek(headersLength, SeekOrigin.Begin);
                    byte[] fileBytes = FileHandling.GetBufferSize(ciphertext);
                    MemoryEncryption.DecryptByteArray(ref key);
                    if (Globals.EncryptionAlgorithm == (int)Cipher.XChaCha20 || Globals.EncryptionAlgorithm == (int)Cipher.XSalsa20)
                    {
                        StreamCiphers.Decrypt(plaintext, ciphertext, fileBytes, nonce, key, bgwDecryption);
                    }
                    else if (Globals.EncryptionAlgorithm == (int)Cipher.AesCBC)
                    {
                        AesAlgorithms.DecryptAesCBC(plaintext, ciphertext, fileBytes, nonce, key, bgwDecryption);
                    }
                }
                Utilities.ZeroArray(key);
                CompleteDecryption(filePath, decryptedFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileEncryptionExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to decrypt the file.");
                Utilities.ZeroArray(key);
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
