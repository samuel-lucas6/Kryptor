using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public static class Argon2Benchmark
    {
        public static void RunBenchmark(bool speedMode)
        {
            const int numberOfTests = 50;
            int[] benchmarkTimes = new int[numberOfTests];
            GetBenchmarkInputs(out byte[] passwordBytes, out byte[] keyfileBytes, out byte[] salt, out byte[] associatedData);
            Globals.Parallelism = Constants.DefaultParallelism;
            Globals.Iterations = Constants.DefaultIterations;
            var memorySize = new List<int>();
            if (speedMode == true)
            {
                // Start benchmark at 10 MiB memory size
                memorySize.Add(10240);
            }
            else
            {
                // Start benchmark at 50 MiB memory size
                memorySize.Add(51200);
            }
            for (int i = 0; i < numberOfTests; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                KeyDerivation.DeriveKeys(passwordBytes, keyfileBytes, salt, associatedData, Globals.Parallelism, memorySize[i], Globals.Iterations);
                stopwatch.Stop();
                benchmarkTimes[i] = Convert.ToInt32(stopwatch.ElapsedMilliseconds);
                // Increment memory size by 5 MiB
                memorySize.Add(memorySize[i] + 5120);
            }
            CalculateMemorySize(benchmarkTimes, memorySize, speedMode);
        }

        private static void GetBenchmarkInputs(out byte[] passwordBytes, out byte[] keyfileBytes, out byte[] salt, out byte[] associatedData)
        {
            char[] password = "K3jmrGo#aysfPs!BwKd@BKw&2T".ToCharArray();
            passwordBytes = FileEncryption.GetPasswordBytes(password);
            keyfileBytes = null;
            salt = Generate.Salt();
            associatedData = HashingAlgorithms.Blake2("Benchmark");
        }

        private static void CalculateMemorySize(int[] benchmarkTimes, List<int> memorySize, bool speedMode)
        {
            int i = 0;
            int recommendedMemorySize = Constants.DefaultMemorySize;
            int delayPerFile = 250;
            if (speedMode == false)
            {
                delayPerFile = 500;
            }
            foreach (int timeElapsed in benchmarkTimes)
            {
                // Recommended memory size is the the closest to the selected delay in ms
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
                    benchmarkResults.Add($"{memorySize[i] / Constants.Mebibyte} MiB : {benchmarkTimes[i]} ms");
                }
                benchmarkResults.Add(Environment.NewLine);
                benchmarkResults.Add($"Recommended Memory Size: {Invariant.ToString(recommendedMemorySize / Constants.Mebibyte)} MiB");
                benchmarkResults.Add($"This memory size was chosen because it was <= {delayPerFile} ms. This is the delay per file that it takes for Argon2 to derive an encryption key and HMAC key. You can speed up key derivation by lowering the memory size, but this will decrease your security. For more information about Argon2, please read the documentation: https://kryptor.co.uk/Key Derivation.html.");
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
            try
            {
                // Prevent Argon2 benchmark reoccuring automatically
                const string firstRunFile = "first run.tmp";
                string firstRunFilePath = Path.Combine(Constants.KryptorDirectory, firstRunFile);
                if (File.Exists(firstRunFilePath))
                {
                    File.Delete(firstRunFilePath);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, $"Unable to delete {Constants.KryptorDirectory}\\first run.tmp. Please manually delete this file to prevent the Argon2 benchmark from automatically running again.");
            }
        }

        public static void TestArgon2Parameters()
        {
            GetBenchmarkInputs(out byte[] passwordBytes, out byte[] keyfileBytes, out byte[] salt, out byte[] associatedData);
            var stopwatch = Stopwatch.StartNew();
            KeyDerivation.DeriveKeys(passwordBytes, keyfileBytes, salt, associatedData, Globals.Parallelism, Globals.MemorySize, Globals.Iterations);
            stopwatch.Stop();
            DisplayMessage.InformationMessageBox($"{Convert.ToInt32(stopwatch.ElapsedMilliseconds)} ms delay per file.", "Argon2 Parameter Results");
        }
    }
}
