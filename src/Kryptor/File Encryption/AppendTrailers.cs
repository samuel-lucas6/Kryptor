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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace Kryptor
{
    public static class AppendTrailers
    {
        public static bool WriteTrailers(string encryptedFilePath, byte[] salt, byte[] nonce)
        {
            try
            {
                NullChecks.ByteArray(salt);
                NullChecks.ByteArray(nonce);
                using (var encryptedFile = new FileStream(encryptedFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    encryptedFile.Write(salt, 0, salt.Length);
                    encryptedFile.Write(nonce, 0, nonce.Length);
                }
                bool success = AppendArgon2Parameters(encryptedFilePath);
                return success;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(encryptedFilePath, ex.GetType().Name, "Unable to append salt/nonce to the encrypted file. This data is required for decryption of the file. The original file will not be overwritten/deleted.");
                return false;
            }
        }

        private static bool AppendArgon2Parameters(string encryptedFilePath)
        {
            try
            {
                ConvertArgon2Parameters(encryptedFilePath, out byte[] parallelismBytes, out byte[] memorySizeBytes, out byte[] iterationsBytes);
                using (var encryptedFile = new FileStream(encryptedFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    encryptedFile.Write(parallelismBytes, 0, parallelismBytes.Length);
                    encryptedFile.Write(memorySizeBytes, 0, memorySizeBytes.Length);
                    encryptedFile.Write(iterationsBytes, 0, iterationsBytes.Length);
                }
                return true;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(encryptedFilePath, ex.GetType().Name, "Unable to store Argon2 parameters in the encrypted file.");
                return false;
            }
        }

        private static void ConvertArgon2Parameters(string encryptedFilePath, out byte[] parallelismBytes, out byte[] memorySizeBytes, out byte[] iterationsBytes)
        {
            string parallelism = Constants.ParallelismFlag + Invariant.ToString(Globals.Parallelism);
            string memorySize = Constants.MemorySizeFlag + Invariant.ToString(Globals.MemorySize);
            string iterations = Constants.IterationsFlag + Invariant.ToString(Globals.Iterations) + Constants.EndFlag;
            var parameters = ReadTrailers.GetParametersBytes(encryptedFilePath, parallelism, memorySize, iterations);
            parallelismBytes = parameters.Item1;
            memorySizeBytes = parameters.Item2;
            iterationsBytes = parameters.Item3;
        }
    }
}
