using System;
using System.Collections.Generic;
using System.Diagnostics;

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

namespace KryptorCLI
{
    public static class Argon2Benchmark
    {
        public static void PerformBenchmark(int delayPerFile)
        {
            Console.WriteLine();
            RunBenchmark(delayPerFile);
            KryptorSettings.SaveSettings();
            // Deallocate RAM for Argon2
            GC.Collect();
            Console.WriteLine();
            Console.WriteLine($"Argon2 will use a memory size of {Invariant.ToString(Globals.MemorySize / Constants.Mebibyte)} MiB. This value can be changed in the settings.");
        }

        private static void RunBenchmark(int delayPerFile)
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
                Console.WriteLine($"{memorySize[i] / Constants.Mebibyte} MiB = {benchmarkTimes[i]} ms");
            }
            CalculateMemorySize(benchmarkTimes, memorySize, delayPerFile);
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

        private static void CalculateMemorySize(int[] benchmarkTimes, List<int> memorySize, int delayPerFile)
        {
            int i = 0;
            int recommendedMemorySize = Constants.DefaultMemorySize;
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
        }

        public static void TestArgon2Parameters()
        {
            GetBenchmarkInputs(out byte[] passwordBytes, out byte[] salt);
            int elapsedTime = 0;
            // Discard first benchmark time due to timer issues
            for (int i = 0; i < 2; i++)
            {
                elapsedTime = GetMemorySizeDelay(passwordBytes, salt, Globals.Iterations, Globals.MemorySize);
            }
            Console.WriteLine();
            Console.WriteLine($"Memory size: {Globals.MemorySize / Constants.Mebibyte} MiB");
            Console.WriteLine($"Iterations: {Globals.Iterations}");
            Console.WriteLine($"Delay per file: {elapsedTime} ms");
        }
    }
}
