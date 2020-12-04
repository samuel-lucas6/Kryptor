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

namespace KryptorGUI
{
    public static class Encryption
    {
        public static void InitializeEncryption(string filePath, byte[] passwordBytes, BackgroundWorker bgwEncryption)
        {
            string encryptedFilePath = GetEncryptedFilePath(filePath);
            byte[] salt = Generate.Salt();
            byte[] nonce = Generate.Nonce();
            var keys = KeyDerivation.DeriveKeys(passwordBytes, salt, Globals.Iterations, Globals.MemorySize);
            EncryptFile(filePath, encryptedFilePath, salt, nonce, keys, bgwEncryption);        
        }

        private static string GetEncryptedFilePath(string filePath)
        {
            if (Globals.AnonymousRename == true)
            {
                bool success = OriginalFileName.AppendOriginalFileName(filePath);
                if (success == true)
                {
                    return AnonymousRename.GetAnonymousFileName(filePath) + Constants.EncryptedExtension;
                }
            }
            return filePath + Constants.EncryptedExtension;
        }

        private static void EncryptFile(string filePath, string encryptedFilePath, byte[] salt, byte[] nonce, (byte[], byte[]) keys, BackgroundWorker bgwEncryption)
        {
            try
            {
                using (var ciphertext = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                using (var plaintext = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                {
                    WriteFileHeaders.WriteHeaders(ciphertext, salt, nonce);
                    // Store headers length to correct percentage calculation
                    long headersLength = ciphertext.Position;
                    byte[] fileBytes = FileHandling.GetBufferSize(plaintext);
                    MemoryEncryption.DecryptByteArray(ref keys.Item1);
                    if (Globals.EncryptionAlgorithm == (int)Cipher.XChaCha20 || Globals.EncryptionAlgorithm == (int)Cipher.XSalsa20)
                    {
                        StreamCiphers.Encrypt(plaintext, ciphertext, headersLength, fileBytes, nonce, keys.Item1, bgwEncryption);
                    }
                    else if (Globals.EncryptionAlgorithm == (int)Cipher.AesCBC)
                    {
                        AesAlgorithms.EncryptAesCBC(plaintext, ciphertext, headersLength, fileBytes, nonce, keys.Item1, bgwEncryption);
                    }
                }
                Utilities.ZeroArray(keys.Item1);
                CompleteEncryption(filePath, encryptedFilePath, keys.Item2);
            }
            catch (Exception ex) when (ExceptionFilters.FileEncryptionExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to encrypt the file.");
                FileHandling.DeleteFile(encryptedFilePath);
                Utilities.ZeroArray(keys.Item1);
                Utilities.ZeroArray(keys.Item2);
            }
        }

        private static void CompleteEncryption(string filePath, string encryptedFilePath, byte[] macKey)
        {
            // Calculate and append MAC
            bool fileSigned = FileAuthentication.SignFile(encryptedFilePath, macKey);
            Utilities.ZeroArray(macKey);
            if (fileSigned == true && Globals.OverwriteFiles == true)
            {
                FileHandling.OverwriteFile(filePath, encryptedFilePath);
            }
            FileHandling.MakeFileReadOnly(encryptedFilePath);
            GetEncryptionResult(filePath, fileSigned);
        }

        private static void GetEncryptionResult(string filePath, bool fileSigned)
        {
            string fileName = Path.GetFileName(filePath);
            if (fileSigned == true)
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
