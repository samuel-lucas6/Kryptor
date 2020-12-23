using Sodium;
using System;
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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace KryptorCLI
{
    public static class Encryption
    {
        public static void InitializeEncryption(string filePath, byte[] passwordBytes)
        {
            string encryptedFilePath = GetEncryptedFilePath(filePath);
            byte[] salt = Generate.Salt();
            (byte[] encryptionKey, byte[] macKey) = KeyDerivation.DeriveKeys(passwordBytes, salt, Globals.Iterations, Globals.MemorySize);
            EncryptFile(filePath, encryptedFilePath, salt, encryptionKey, macKey);        
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

        private static void EncryptFile(string filePath, string encryptedFilePath, byte[] salt, byte[] encryptionKey, byte[] macKey)
        {
            try
            {
                using (var ciphertext = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                using (var plaintext = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, Constants.FileBufferSize, FileOptions.SequentialScan))
                {
                    WriteFileHeaders.WriteHeaders(ciphertext, salt);
                    byte[] fileBytes = FileHandling.GetBufferSize(plaintext.Length);
                    // Generate a counter starting at 0
                    byte[] counter = Generate.Counter();
                    int bytesRead;
                    MemoryEncryption.DecryptByteArray(ref encryptionKey);
                    while ((bytesRead = plaintext.Read(fileBytes, 0, fileBytes.Length)) > 0)
                    {
                        byte[] encryptedBytes = StreamEncryption.EncryptXChaCha20(fileBytes, counter, encryptionKey);
                        ciphertext.Write(encryptedBytes, 0, bytesRead);
                        counter = Sodium.Utilities.Increment(counter);
                    }
                }
                Utilities.ZeroArray(encryptionKey);
                CompleteEncryption(filePath, encryptedFilePath, macKey);
            }
            catch (Exception ex) when (ExceptionFilters.FileEncryptionExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.Error(filePath, ex.GetType().Name, "Unable to encrypt the file.");
                FileHandling.DeleteFile(encryptedFilePath);
                Utilities.ZeroArray(encryptionKey);
                Utilities.ZeroArray(macKey);
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
                Console.WriteLine($"{fileName}: File encryption successful.");
                Globals.SuccessfulCount += 1;
            }
            else
            {
                Console.WriteLine($"{fileName}: File encryption failed.");
            }
        }
    }
}
