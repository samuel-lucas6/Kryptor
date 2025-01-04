/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2025 Samuel Lucas

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
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Geralt;

namespace Kryptor;

public static class PassphrasePrompt
{
    public static Span<byte> GetNewPassphrase(Span<byte> passphrase)
    {
        passphrase = passphrase.Length switch
        {
            0 => EnterNewPassphrase(),
            1 when ConstantTime.Equals(passphrase, " "u8) => UseRandomPassphrase(),
            _ => passphrase
        };
        const int maxAscii = 127;
        bool lessThanEqualTo = true;
        foreach (byte b in passphrase) {
            lessThanEqualTo &= Convert.ToBoolean(((b - maxAscii - 1) >> 31) & 1);
        }
        if (!lessThanEqualTo) {
            DisplayMessage.WriteLine("WARNING: Using non-ASCII characters in a passphrase may cause problems decrypting files/private keys.", ConsoleColor.DarkYellow);
        }
        return passphrase;
    }

    private static Span<byte> EnterNewPassphrase()
    {
        Console.WriteLine("Enter a passphrase (leave empty for a random one):");
        Span<byte> passphrase = GetPassphrase();
        if (passphrase.Length == 0) {
            return UseRandomPassphrase();
        }
        RetypeNewPassphrase(passphrase);
        return passphrase;
    }

    private static void RetypeNewPassphrase(Span<byte> passphrase)
    {
        Console.WriteLine("Retype passphrase:");
        Span<byte> retypedPassphrase = GetPassphrase();
        if (retypedPassphrase.Length == 0 || !ConstantTime.Equals(passphrase, retypedPassphrase)) {
            DisplayMessage.Error("The passphrases don't match.");
            Environment.Exit(Constants.ErrorCode);
        }
        CryptographicOperations.ZeroMemory(retypedPassphrase);
    }

    private static Span<byte> UseRandomPassphrase()
    {
        char[] passphrase = SecureRandom.GetPassphrase(wordCount: 8, separatorChar: '-', capitalize: false, includeNumber: false);
        Console.Write("Randomly generated passphrase: ");
        Console.WriteLine(passphrase);
        Console.WriteLine();
        return Encoding.UTF8.GetBytes(passphrase);
    }

    public static Span<byte> EnterYourPassphrase(bool isPrivateKey = false)
    {
        Console.WriteLine(isPrivateKey == false ? "Enter your passphrase:" : "Enter your private key passphrase:");
        Span<byte> passphrase = GetPassphrase();
        if (passphrase.Length == 0) {
            DisplayMessage.Error("You didn't enter a passphrase.");
            Environment.Exit(Constants.ErrorCode);
        }
        return passphrase;
    }

    private static Span<byte> GetPassphrase()
    {
        var passphrase = new List<char>();
        ConsoleKeyInfo consoleKeyInfo;
        while ((consoleKeyInfo = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter) {
            if (!char.IsControl(consoleKeyInfo.KeyChar)) {
                passphrase.Add(consoleKeyInfo.KeyChar);
            }
            else if (consoleKeyInfo.Key is ConsoleKey.Backspace or ConsoleKey.Delete && passphrase.Count > 0) {
                passphrase.RemoveAt(passphrase.Count - 1);
            }
        }
        Console.WriteLine();
        return Encoding.UTF8.GetBytes(passphrase.ToArray());
    }
}
