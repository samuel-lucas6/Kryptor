/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2022 Samuel Lucas

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

using System;
using System.Collections.Generic;
using System.Globalization;
using Sodium;

namespace KryptorCLI;

public static class PassphraseGenerator
{
    public static char[] GetRandomPassphrase(int wordCount)
    {
        string[] wordlist = GetWordlist();
        List<string> words = GetRandomWords(wordlist, wordCount);
        return FormatPassphrase(words, wordCount);
    }

    private static string[] GetWordlist()
    {
        return Properties.Resources.wordlist.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static List<string> GetRandomWords(string[] wordlist, int wordCount)
    {
        var words = new List<string>();
        var textInfo = new CultureInfo("en-US", useUserOverride: false).TextInfo;
        for (int i = 0; i < wordCount; i++)
        {
            int randomIndex = SodiumCore.GetRandomNumber(wordlist.Length);
            words.Add(textInfo.ToTitleCase(wordlist[randomIndex]));
        }
        return words;
    }

    private static char[] FormatPassphrase(List<string> words, int wordCount)
    {
        var passphrase = new List<char>();
        for (int i = 0; i < wordCount; i++)
        {
            passphrase.AddRange(words[i]);
            if (i != wordCount - 1) { passphrase.Add('-'); }
        }
        return passphrase.ToArray();
    }
}