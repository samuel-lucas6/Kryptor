using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public static class Argon2Benchmark
    {
        public static void RunBenchmark(bool speedMode)
        {
            // Benchmark up to 300 MiB
            const int testCount = 51;
            int[] benchmarkTimes = new int[testCount];
            GetBenchmarkInputs(out byte[] passwordBytes, out byte[] salt);
            Globals.Iterations = Constants.DefaultIterations;
            // Start benchmark at 50 MiB
            int initialMemorySize = 50 * Constants.Mebibyte;
            var memorySize = new List<int>
            {
                initialMemorySize,
            };
            // Increment memory size by 5 MiB
            int incrementMemorySize = 5 * Constants.Mebibyte;
            for (int i = 0; i < testCount; i++)
            {
                int elapsedTime = GetMemorySizeDelay(passwordBytes, salt, Globals.Iterations, memorySize[i]);
                // Run first memory size twice due to timer issues
                if (i == 0)
                {
                    elapsedTime = GetMemorySizeDelay(passwordBytes, salt, Globals.Iterations, memorySize[i]);
                }
                benchmarkTimes[i] = elapsedTime;
                memorySize.Add(memorySize[i] + incrementMemorySize);
            }
            CalculateMemorySize(benchmarkTimes, memorySize, speedMode);
        }

        private static int GetMemorySizeDelay(byte[] passwordBytes, byte[] salt, int iterations, int memorySize)
        {
            var stopwatch = Stopwatch.StartNew();
            KeyDerivation.DeriveKeys(passwordBytes, salt, iterations, memorySize);
            stopwatch.Stop();
            return Convert.ToInt32(stopwatch.ElapsedMilliseconds);
        }

        private static void GetBenchmarkInputs(out byte[] passwordBytes, out byte[] salt)
        {
            char[] password = "K3jmrGo#aysfPs!BwKd@BKw&2T".ToCharArray();
            passwordBytes = FileEncryption.GetPasswordBytes(password);
            salt = Generate.Salt();
        }

        private static void CalculateMemorySize(int[] benchmarkTimes, List<int> memorySize, bool speedMode)
        {
            int i = 0;
            int recommendedMemorySize = Constants.DefaultMemorySize;
            int delayPerFile = 250;
            if (speedMode == true)
            {
                delayPerFile = 150;
            }
            foreach (int timeElapsed in benchmarkTimes)
            {
                // Recommended memory size is the closest to the selected delay in ms
                if (timeElapsed <= delayPerFile)
                {
                    recommendedMemorySize = memorySize[i];
                }
                i++;
            }
            Globals.MemorySize = recommendedMemorySize;
            StoreBenchmarkResults(benchmarkTimes, memorySize, recommendedMemorySize, delayPerFile);
        }

        private static void StoreBenchmarkResults(int[] benchmarkTimes, List<int> memorySize, int recommendedMemorySize, int delayPerFile)
        {
            try
            {
                var benchmarkResults = new List<string>();
                for (int i = 0; i < benchmarkTimes.Length; i++)
                {
                    benchmarkResults.Add($"{memorySize[i] / Constants.Mebibyte} MiB = {benchmarkTimes[i]} ms");
                }
                benchmarkResults.Add(string.Empty);
                benchmarkResults.Add($"Recommended Memory Size: {Invariant.ToString(recommendedMemorySize / Constants.Mebibyte)} MiB");
                benchmarkResults.Add($"This memory size was chosen because it was <= {delayPerFile} ms. This is the delay per file that it takes for Argon2 to derive an encryption key and MAC key. You can speed up key derivation by lowering the memory size, but this will decrease your security. For more information about Argon2, please read the documentation (https://kryptor.co.uk/Key Derivation.html).");
                string benchmarkFilePath = Path.Combine(Constants.KryptorDirectory, "benchmark.txt");   
                File.WriteAllLines(benchmarkFilePath, benchmarkResults);
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to store benchmark results.");
            }
        }

        public static void DeleteFirstRunFile()
        {
            const string firstRunFile = "first run.tmp";
            try
            {
                // Prevent Argon2 benchmark reoccuring automatically
                string firstRunFilePath = Path.Combine(Constants.KryptorDirectory, firstRunFile);
                if (File.Exists(firstRunFilePath))
                {
                    File.Delete(firstRunFilePath);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, $"Unable to delete {Constants.KryptorDirectory}\\{firstRunFile}. Please manually delete this file to prevent the Argon2 benchmark from automatically running again.");
            }
        }

        public static int TestArgon2Parameters()
        {
            GetBenchmarkInputs(out byte[] passwordBytes, out byte[] salt);
            int elapsedTime = 0;
            // Discard first benchmark time due to timer issues
            for (int i = 0; i < 2; i++)
            {
                elapsedTime = GetMemorySizeDelay(passwordBytes, salt, Globals.Iterations, Globals.MemorySize);
            }
            return elapsedTime;
        }
    }
}
