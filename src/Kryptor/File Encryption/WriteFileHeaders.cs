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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace Kryptor
{
    public static class WriteFileHeaders
    {
        public static void WriteHeaders(FileStream ciphertext, byte[] salt, byte[] nonce)
        {
            NullChecks.FileHeaders(ciphertext, salt, nonce);
            ConvertArgon2Parameters(out byte[] memorySize, out byte[] iterations, out byte[] endFlag);
            ciphertext.Write(memorySize, 0, memorySize.Length);
            ciphertext.Write(iterations, 0, iterations.Length);
            ciphertext.Write(endFlag, 0, endFlag.Length);
            ciphertext.Write(salt, 0, salt.Length);
            ciphertext.Write(nonce, 0, nonce.Length);
        }

        private static void ConvertArgon2Parameters(out byte[] memorySizeBytes, out byte[] iterationsBytes, out byte[] endFlag)
        {
            string memorySize = Constants.MemorySizeFlag + Invariant.ToString(Globals.MemorySize);
            string iterations = Constants.IterationsFlag + Invariant.ToString(Globals.Iterations);
            var parameters = GetParametersBytes(memorySize, iterations);
            memorySizeBytes = parameters.Item1;
            iterationsBytes = parameters.Item2;
            endFlag = parameters.Item3;
        }

        public static (byte[], byte[], byte[]) GetParametersBytes(string memorySize, string iterations)
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
                DisplayMessage.ErrorResultsText(string.Empty, ex.GetType().Name, "Unable to convert Argon2 parameters to bytes.");
                return (null, null, null);
            }
        }
    }
}
