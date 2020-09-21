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
        public static void InitializeDecryption(string filePath, byte[] passwordBytes, byte[] keyfileBytes, byte[] associatedData, BackgroundWorker bgwDecryption)
        {
            int parametersLength = 0;
            int[] argon2Parameters = ReadTrailers.ReadArgon2Parameters(filePath);
            if (argon2Parameters != null)
            {
                // argon2Parameters[3] = Length of parameter bytes in the file
                parametersLength = argon2Parameters[3];
            }
            byte[] nonce = ReadTrailers.ReadNonce(filePath, parametersLength);
            byte[] salt = ReadTrailers.ReadSalt(filePath, nonce, parametersLength);
            (byte[], byte[]) keys;
            if (argon2Parameters != null)
            {
                keys = KeyDerivation.DeriveKeys(passwordBytes, keyfileBytes, salt, associatedData, argon2Parameters[0], argon2Parameters[1], argon2Parameters[2]);
            }
            else
            {
                // If the Argon2 parameters are in the file but couldn't be read, then this will fail
                keys = KeyDerivation.DeriveKeys(passwordBytes, keyfileBytes, salt, associatedData, Globals.Parallelism, Globals.MemorySize, Globals.Iterations);
            }
            CheckForTampering(filePath, keys, nonce, parametersLength, bgwDecryption);
        }

        private static void CheckForTampering(string filePath, (byte[], byte[]) keys, byte[] nonce, int parametersLength, BackgroundWorker bgwDecryption)
        {
            bool fileTampered = FileAuthentication.AuthenticateFile(filePath, keys.Item2);
            Utilities.ZeroArray(keys.Item2);
            if (fileTampered == false)
            {
                DecryptFile(filePath, parametersLength, nonce, keys.Item1, bgwDecryption);
            }
            else
            {
                Globals.ResultsText += $"{Path.GetFileName(filePath)}: Incorrect password/keyfile, wrong encryption algorithm, or this file has been tampered with.{Environment.NewLine}";
                Utilities.ZeroArray(keys.Item1);
            }
        }

        private static void DecryptFile(string filePath, int parametersLength, byte[] nonce, byte[] key, BackgroundWorker bgwDecryption)
        {
            string backupFilePath = Regex.Replace(filePath, Constants.EncryptedExtension, ".backup");
            string decryptedFilePath = Regex.Replace(filePath, Constants.EncryptedExtension, string.Empty);
            try
            {
                // Get length of bytes to remove before decryption (salt/nonce, etc)
                int trailersLength = ReadTrailers.GetTrailersLength(nonce.Length, parametersLength);
                // Backup data required for decryption in case something goes wrong
                DecryptionBackup.BackupTrailers(filePath, backupFilePath, trailersLength);
                using (var plaintext = new FileStream(decryptedFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                using (var ciphertext = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    // Remove trailers from the encrypted file
                    ciphertext.SetLength(ciphertext.Length - trailersLength);
                    byte[] fileBytes = new byte[4096];
                    if (fileBytes.Length > ciphertext.Length)
                    {
                        fileBytes = new byte[ciphertext.Length];
                    }
                    MemoryEncryption.DecryptByteArray(ref key);
                    if (Globals.EncryptionAlgorithm == (int)Cipher.XChaCha20 | Globals.EncryptionAlgorithm == (int)Cipher.XSalsa20)
                    {
                        StreamCiphers.Decrypt(plaintext, ciphertext, fileBytes, nonce, key, bgwDecryption);
                    }
                    else if (Globals.EncryptionAlgorithm == (int)Cipher.AesCBC)
                    {
                        AesAlgorithms.DecryptAesCBC(plaintext, ciphertext, fileBytes, nonce, key, bgwDecryption);
                    }
                    else if (Globals.EncryptionAlgorithm == (int)Cipher.AesCTR)
                    {
                        AesAlgorithms.AesCTR(ciphertext, plaintext, fileBytes, nonce, key, bgwDecryption);
                    }
                }
                Utilities.ZeroArray(key);
                CompleteDecryption(filePath, decryptedFilePath, backupFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileEncryptionExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to decrypt the file.");
                DecryptionBackup.RestoreTrailers(filePath, backupFilePath);
                FileEncryption.EncryptionDecryptionFailed(key, null, decryptedFilePath);
            }
        }

        private static void CompleteDecryption(string filePath, string decryptedFilePath, string backupFilePath)
        {
            // Deanonymise file name (if anonymous rename is enabled)
            OriginalFileName.RestoreOriginalFileName(decryptedFilePath);
            DeleteEncryptedFile(filePath, backupFilePath);
            Globals.ResultsText += Path.GetFileName(filePath) + ": File decryption successful." + Environment.NewLine;
            Globals.SuccessfulCount += 1;
        }

        private static void DeleteEncryptedFile(string filePath, string backupFilePath)
        {
            try
            {
                File.Delete(filePath);
                File.SetAttributes(backupFilePath, FileAttributes.Normal);
                File.Delete(backupFilePath);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to delete the encrypted file and/or backup file.");
            }
        }
    }
}
