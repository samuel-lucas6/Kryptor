using System;
using System.IO;
using System.Text;

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
    public static class ReadFileHeaders
    {
        public static (int memorySize, int iterations, int parametersLength) ReadArgon2Parameters(string filePath)
        {
            string memorySize = string.Empty, iterations = string.Empty;
            // Read the parameter strings from the file
            RetrieveArgon2Parameters(filePath, ref memorySize, ref iterations);
            if (!string.IsNullOrEmpty(memorySize) && !string.IsNullOrEmpty(iterations))
            {
                return GetParameterValues(memorySize, iterations);
            }
            else
            {
                return (0, 0, 0);
            }
        }

        private static void RetrieveArgon2Parameters(string filePath, ref string memorySize, ref string iterations)
        {
            try
            {
                // Read the first line of the file
                using var streamReader = new StreamReader(filePath, true);
                string firstLine = streamReader.ReadLine();
                int memorySizeIndex = firstLine.IndexOf(Constants.MemorySizeFlag, StringComparison.Ordinal);
                if (memorySizeIndex != -1)
                {
                    int iterationsIndex = firstLine.IndexOf(Constants.IterationsFlag, StringComparison.Ordinal);
                    int endIndex = firstLine.IndexOf(Constants.EndFlag, StringComparison.Ordinal);
                    // If the strings are found on the line
                    if (memorySizeIndex != -1 && iterationsIndex != -1 && endIndex != -1)
                    {
                        memorySize = firstLine.Substring(memorySizeIndex, iterationsIndex - memorySizeIndex);
                        iterations = firstLine.Substring(iterationsIndex, endIndex - iterationsIndex);
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex) || ExceptionFilters.CharacterEncodingExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.Error(filePath, ex.GetType().Name, "Unable to read Argon2 parameters from the file.");
            }
        }

        private static (int memorySize, int iterations, int parametersLength) GetParameterValues(string memorySize, string iterations)
        {
            // Get the number of bytes to skip when reading
            int parametersLength = GetParametersLength(memorySize, iterations);
            // Get parameter values - remove file flags (e.g. |m=value)
            memorySize = RemoveParameterFlag(memorySize);
            iterations = RemoveParameterFlag(iterations);
            return (Invariant.ToInt(memorySize), Invariant.ToInt(iterations), parametersLength);
        }

        private static int GetParametersLength(string memorySize, string iterations)
        {
            int parametersLength = 0;
            var parameters = GetParametersBytes(memorySize, iterations);
            if (parameters.Item1 != null && parameters.Item2 != null && parameters.Item3 != null)
            {
                parametersLength = parameters.Item1.Length + parameters.Item2.Length + parameters.Item3.Length;
            }
            return parametersLength;
        }

        private static (byte[], byte[], byte[]) GetParametersBytes(string memorySize, string iterations)
        {
            try
            {
                Encoding encoding = Encoding.UTF8;
                byte[] memorySizeBytes = encoding.GetBytes(memorySize);
                byte[] iterationsBytes = encoding.GetBytes(iterations);
                byte[] endFlagBytes = encoding.GetBytes(Constants.EndFlag);
                return (memorySizeBytes, iterationsBytes, endFlagBytes);
            }
            catch (Exception ex) when (ExceptionFilters.CharacterEncodingExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.Error(ex.GetType().Name, "Unable to convert Argon2 parameters to bytes.");
                return (null, null, null);
            }
        }

        private static string RemoveParameterFlag(string parameter)
        {
            // All flags are the same length except the end flag
            int flagLength = Constants.MemorySizeFlag.Length;
            return parameter.Substring(flagLength, parameter.Length - flagLength);
        }

        public static byte[] ReadSalt(string filePath, int parametersLength)
        {
            return ReadHeader(filePath, Constants.SaltLength, parametersLength);
        }

        private static byte[] ReadHeader(string filePath, int headerLength, int offset)
        {
            try
            {
                byte[] header = new byte[headerLength];
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, Constants.FileBufferSize, FileOptions.RandomAccess))
                {
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    fileStream.Read(header, 0, header.Length);
                }
                return header;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.Error(filePath, ex.GetType().Name, "Unable to read salt from the selected file.");
                return null;
            }
        }
    }
}
