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
    public static class Encryption
    {
        public static void InitializeEncryption(string filePath, byte[] passwordBytes, byte[] keyfileBytes, byte[] associatedData, BackgroundWorker bgwEncryption)
        {
            string encryptedFilePath = GetEncryptedFilePath(filePath);
            byte[] salt = Generate.Salt();
            byte[] nonce = Generate.Nonce();
            var keys = KeyDerivation.DeriveKeys(passwordBytes, keyfileBytes, salt, associatedData, Globals.Parallelism, Globals.MemorySize, Globals.Iterations);
            EncryptFile(filePath, encryptedFilePath, salt, nonce, keys, bgwEncryption);        
        }

        private static string GetEncryptedFilePath(string filePath)
        {
            string encryptedFilePath = filePath + Constants.EncryptedExtension;
            if (Globals.AnonymousRename == true)
            {
                bool success = OriginalFileName.AppendOriginalFileName(filePath);
                if (success == true)
                {
                    encryptedFilePath = AnonymousRename.GetAnonymousFileName(filePath) + Constants.EncryptedExtension;
                }
            }
            return encryptedFilePath;
        }

        private static void EncryptFile(string filePath, string encryptedFilePath, byte[] salt, byte[] nonce, (byte[], byte[]) keys, BackgroundWorker bgwEncryption)
        {
            try
            {
                using (var ciphertext = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                using (var plaintext = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    byte[] fileBytes = new byte[4096];
                    if (fileBytes.Length > plaintext.Length)
                    {
                        fileBytes = new byte[plaintext.Length];
                    }
                    MemoryEncryption.DecryptByteArray(ref keys.Item1);
                    if (Globals.EncryptionAlgorithm == (int)Cipher.XChaCha20 | Globals.EncryptionAlgorithm == (int)Cipher.XSalsa20)
                    {
                        StreamCiphers.Encrypt(plaintext, ciphertext, fileBytes, nonce, keys.Item1, bgwEncryption);
                    }
                    else if (Globals.EncryptionAlgorithm == (int)Cipher.AesCBC)
                    {
                        AesAlgorithms.EncryptAesCBC(plaintext, ciphertext, fileBytes, nonce, keys.Item1, bgwEncryption);
                    }
                    else if (Globals.EncryptionAlgorithm == (int)Cipher.AesCTR)
                    {
                        AesAlgorithms.AesCTR(plaintext, ciphertext, fileBytes, nonce, keys.Item1, bgwEncryption);
                    }
                }
                Utilities.ZeroArray(keys.Item1);
                CompleteEncryption(filePath, encryptedFilePath, salt, nonce, keys.Item2);
            }
            catch (Exception ex) when (ExceptionFilters.FileEncryptionExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to encrypt the file.");
                FileEncryption.EncryptionDecryptionFailed(keys.Item1, keys.Item2, encryptedFilePath);
            }
        }

        private static void CompleteEncryption(string filePath, string encryptedFilePath, byte[] salt, byte[] nonce, byte[] hmacKey)
        {
            // Append salt/nonce
            bool trailersAppended = AppendTrailers.WriteTrailers(encryptedFilePath, salt, nonce);
            bool fileSigned = false;
            if (trailersAppended == true)
            {
                // Calculate and append HMAC
                fileSigned = FileAuthentication.SignFile(encryptedFilePath, hmacKey);
                if (fileSigned == true)
                {
                    OverwriteOriginalFile(filePath, encryptedFilePath);
                }
            }
            Utilities.ZeroArray(hmacKey);
            MakeFileReadOnly(encryptedFilePath);
            GetEncryptionResult(filePath, trailersAppended, fileSigned);
        }

        private static void OverwriteOriginalFile(string filePath, string encryptedFilePath)
        {
            try
            {
                if (Globals.OverwriteFiles == true)
                {
                    // Overwrite the original, unencrypted file
                    File.Copy(encryptedFilePath, filePath, true);
                    File.Delete(filePath);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to overwrite and/or delete the original file.");
            }
        }

        private static void MakeFileReadOnly(string encryptedFilePath)
        {
            try
            {
                File.SetAttributes(encryptedFilePath, FileAttributes.ReadOnly);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorResultsText(encryptedFilePath, ex.GetType().Name, "Unable to make the file read-only.");
            }
        }

        private static void GetEncryptionResult(string filePath, bool trailersAppended, bool fileSigned)
        {
            string fileName = Path.GetFileName(filePath);
            if (trailersAppended == true & fileSigned == true)
            {
                Globals.ResultsText += $"{fileName}: File encryption successful.{Environment.NewLine}";
                Globals.SuccessfulCount += 1;
            }
            else
            {
                Globals.ResultsText += $"{fileName}: File encryption failed.{Environment.NewLine}";
            }
        }
    }
}
