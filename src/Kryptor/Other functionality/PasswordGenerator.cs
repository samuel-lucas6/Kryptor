using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sodium;

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
    public static class PasswordGenerator
    {
        public static char[] GenerateRandomPassword(int length, bool lowercase, bool uppercase, bool numbers, bool symbols)
        {
            const string lowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numberCharacters = "1234567890";
            const string symbolCharacters = "!#$%&*?@+-=^";
            List<char> password = new List<char>();
            while (password.Count < length)
            {
                bool characterAdded = false;
                byte[] characterByte = SodiumCore.GetRandomBytes(1);
                char character = (char)characterByte[0];
                if (lowercase == true)
                {
                    CheckCharacter(lowercaseCharacters, character, ref password, ref characterAdded);
                }
                if (uppercase == true && characterAdded == false)
                {
                    CheckCharacter(uppercaseCharacters, character, ref password, ref characterAdded);
                }
                if (numbers == true && characterAdded == false)
                {
                    CheckCharacter(numberCharacters, character, ref password, ref characterAdded);
                }
                if (symbols == true && characterAdded == false)
                {
                    CheckCharacter(symbolCharacters, character, ref password, ref characterAdded);
                }
            }
            return password.ToArray();
        }

        private static void CheckCharacter(string characterSet, char character, ref List<char> password, ref bool characterAdded)
        {
            if (characterSet.Contains(character))
            {
                password.Add(character);
                characterAdded = true;
            }
        }

        public static char[] GenerateRandomPassphrase(int wordCount, bool uppercase, bool numbers)
        {
            try
            {
                string wordlistFilePath = Path.Combine(Constants.KryptorDirectory, "wordlist.txt");
                if (File.Exists(wordlistFilePath))
                {
                    List<char> passphrase = new List<char>();
                    int wordlistLength = File.ReadLines(wordlistFilePath).Count();
                    int[] lineNumbers = GenerateLineNumbers(wordlistLength, wordCount);
                    string[] words = GetRandomWords(wordlistFilePath, lineNumbers, wordCount, uppercase, numbers);
                    Array.Clear(lineNumbers, 0, lineNumbers.Length);
                    if (words != null)
                    {
                        FormatPassphrase(words, ref passphrase, wordCount);
                        Array.Clear(words, 0, words.Length);
                    }
                    return passphrase.ToArray();
                }
                else
                {
                    File.WriteAllText(wordlistFilePath, Properties.Resources.wordlist);
                    return GenerateRandomPassphrase(wordCount, uppercase, numbers);
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to generate a random passphrase.");
                return Array.Empty<char>();
            }
        }

        private static int[] GenerateLineNumbers(int wordlistLength, int wordCount)
        {
            int[] lineNumbers = new int[wordCount];
            for (int i = 0; i < wordCount; i++)
            {
                byte[] randomBytes = SodiumCore.GetRandomBytes(4);
                uint max = BitConverter.ToUInt32(randomBytes, 0);
                lineNumbers[i] = (int)(wordlistLength * (max / (double)uint.MaxValue));
            }
            return lineNumbers;
        }

        private static string[] GetRandomWords(string wordListFilePath, int[] lineNumbers, int wordCount, bool upperCase, bool numbers)
        {
            try
            {
                string[] words = new string[wordCount];
                for (int i = 0; i < wordCount; i++)
                {
                    words[i] = File.ReadLines(wordListFilePath).Skip(lineNumbers[i]).Take(1).First();
                    // Remove any numbers/spaces on the line
                    words[i] = Regex.Replace(words[i], @"[\d-]", string.Empty).Trim();
                    if (upperCase == true)
                    {
                        words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower(CultureInfo.CurrentCulture));
                    }
                    if (numbers == true)
                    {
                        words[i] += words[i].Length;
                    }
                }
                return words;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex) || ex is RegexMatchTimeoutException)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to retrieve words from the wordlist.");
                return null;
            }
        }

        private static void FormatPassphrase(string[] words, ref List<char> passphrase, int wordCount)
        {
            for (int i = 0; i < wordCount; i++)
            {
                foreach (char character in words[i])
                {
                    passphrase.Add(character);
                }
                // Add word separator symbol
                if (i != wordCount - 1)
                {
                    passphrase.Add('-');
                }
            }
        }
    }
}
