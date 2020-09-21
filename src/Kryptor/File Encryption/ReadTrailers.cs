using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    public static class ReadTrailers
    {
        public static int GetTrailersLength(int nonceLength, int parametersLength)
        {
            // Length of bytes to remove from the end of the encrypted file
            return Constants.HMACLength + Constants.SaltLength + nonceLength + parametersLength;
        }

        public static int[] ReadArgon2Parameters(string filePath)
        {
            string parallelism = string.Empty, memorySize = string.Empty, iterations = string.Empty;
            // Read the parameter strings from the file
            RetrieveArgon2Parameters(filePath, ref parallelism, ref memorySize, ref iterations);
            if (!string.IsNullOrEmpty(parallelism) & !string.IsNullOrEmpty(memorySize) & !string.IsNullOrEmpty(iterations))
            {
                return GetParameterValues(filePath, parallelism, memorySize, iterations);
            }
            else
            {
                return null;
            }
        }

        private static void RetrieveArgon2Parameters(string filePath, ref string parallelism, ref string memorySize, ref string iterations)
        {
            try
            {
                // Read the last 5 lines of the file
                List<string> fileLastLines = File.ReadLines(filePath).Reverse().Take(5).Reverse().ToList();
                foreach (string line in fileLastLines)
                {
                    int parallelismIndex = line.IndexOf(Constants.ParallelismFlag, StringComparison.Ordinal);
                    if (parallelismIndex != -1)
                    {
                        int memorySizeIndex = line.IndexOf(Constants.MemorySizeFlag, StringComparison.Ordinal);
                        int iterationsIndex = line.IndexOf(Constants.IterationsFlag, StringComparison.Ordinal);
                        int endIndex = line.IndexOf(Constants.EndFlag, StringComparison.Ordinal);
                        // If the strings are found on the line
                        if (memorySizeIndex != -1 & iterationsIndex != -1 & endIndex != -1)
                        {
                            parallelism = line.Substring(parallelismIndex, memorySizeIndex - parallelismIndex);
                            memorySize = line.Substring(memorySizeIndex, iterationsIndex - memorySizeIndex);
                            iterations = line.Substring(iterationsIndex, endIndex - iterationsIndex);
                        }
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex) || ExceptionFilters.CharacterEncodingExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to read Argon2 parameters from the file.");
            }
        }

        private static int[] GetParameterValues(string filePath, string parallelism, string memorySize, string iterations)
        {
            // Get the number of bytes to remove the parameters from the file
            int bytesToRemove = GetParametersLength(filePath, parallelism, memorySize, iterations);
            // Get parameter values - remove file flags (e.g. |p=value)
            parallelism = RemoveParameterFlag(parallelism);
            memorySize = RemoveParameterFlag(memorySize);
            iterations = RemoveParameterFlag(iterations);
            int[] parameters = new int[] { Invariant.ToInt(parallelism), Invariant.ToInt(memorySize), Invariant.ToInt(iterations), bytesToRemove };
            return parameters;
        }

        private static string RemoveParameterFlag(string parameter)
        {
            // All flags are the same length except the end flag
            int flagLength = Constants.ParallelismFlag.Length;
            return parameter.Substring(flagLength, parameter.Length - flagLength);
        }

        private static int GetParametersLength(string filePath, string parallelism, string memorySize, string iterations)
        {
            int parametersLength = 0;
            var parameters = GetParametersBytes(filePath, parallelism, memorySize, iterations);
            if (parameters.Item1 != null & parameters.Item2 != null & parameters.Item3 != null & parameters.Item4 != null)
            {
                parametersLength = parameters.Item1.Length + parameters.Item2.Length + parameters.Item3.Length + parameters.Item4.Length;
            }
            return parametersLength;
        }

        public static (byte[], byte[], byte[], byte[]) GetParametersBytes(string filePath, string parallelism, string memorySize, string iterations)
        {
            try
            {
                // Detect encoding for the file
                using (var streamReader = new StreamReader(filePath, true))
                {
                    streamReader.Peek();
                    var charEncoding = streamReader.CurrentEncoding;
                    byte[] parallelismBytes = charEncoding.GetBytes(parallelism);
                    byte[] memorySizeBytes = charEncoding.GetBytes(memorySize);
                    byte[] iterationsBytes = charEncoding.GetBytes(iterations);
                    byte[] endFlagBytes = charEncoding.GetBytes(Constants.EndFlag);
                    return (parallelismBytes, memorySizeBytes, iterationsBytes, endFlagBytes);
                }
            }
            catch (Exception ex) when (ExceptionFilters.CharacterEncodingExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to convert Argon2 parameters to bytes.");
                return (null, null, null, null);
            }
        }

        public static byte[] ReadNonce(string filePath, int parametersLength)
        {
            byte[] nonce = null;
            if (Globals.EncryptionAlgorithm == (int)Cipher.XChaCha20 | Globals.EncryptionAlgorithm == (int)Cipher.XSalsa20)
            {
                nonce = new byte[Constants.XChaChaNonceLength];
            }
            else if (Globals.EncryptionAlgorithm == (int)Cipher.AesCBC | Globals.EncryptionAlgorithm == (int)Cipher.AesCTR)
            {
                nonce = new byte[Constants.AesNonceLength];
            }
            int offset = Constants.HMACLength + nonce.Length + parametersLength;
            nonce = ReadTrailer(filePath, nonce, offset);
            return nonce;
        }

        public static byte[] ReadSalt(string filePath, byte[] nonce, int parametersLength)
        {
            NullChecks.ByteArray(nonce);
            byte[] salt = new byte[Constants.SaltLength];
            int offset = Constants.HMACLength + nonce.Length + salt.Length + parametersLength;
            salt = ReadTrailer(filePath, salt, offset);
            return salt;
        }

        private static byte[] ReadTrailer(string filePath, byte[] trailer, int offset)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fileStream.Seek(fileStream.Length - offset, SeekOrigin.Begin);
                    fileStream.Read(trailer, 0, trailer.Length);
                }
                return trailer;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorResultsText(filePath, ex.GetType().Name, "Unable to read trailer variable (salt or nonce) from the selected file.");
                return null;
            }
        }
    }
}
