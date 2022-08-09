/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
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
using Geralt;

namespace Kryptor;

public static class PasswordPrompt
{
    public static char[] EnterNewPassword()
    {
        Console.WriteLine("Enter a password (leave empty for a random passphrase):");
        char[] password = GetPassword();
        if (password.Length == 0) { return UseRandomPassphrase(); }
        RetypeNewPassword(password);
        return password;
    }

    private static void RetypeNewPassword(char[] password)
    {
        Console.WriteLine("Retype password:");
        char[] retypedPassword = GetPassword();
        if (!Arrays.Compare(password, retypedPassword))
        {
            DisplayMessage.Error("The passwords don't match.");
            Environment.Exit(exitCode: 13);
        }
        Arrays.ZeroMemory(retypedPassword);
    }
    
    public static char[] UseRandomPassphrase()
    {
        char[] password = SecureRandom.GetPassphrase(wordCount: 8);
        Console.Write("Randomly generated passphrase: ");
        Console.WriteLine(password);
        Console.WriteLine();
        return password;
    }

    public static char[] EnterYourPassword(bool isPrivateKey = false)
    {
        Console.WriteLine(isPrivateKey == false ? "Enter your password:" : "Enter your private key password:");
        char[] password = GetPassword();
        if (password.Length != 0) { return password; }
        DisplayMessage.Error("You didn't enter a password.");
        Environment.Exit(exitCode: 13);
        return password;
    }

    private static char[] GetPassword()
    {
        var password = new List<char>();
        ConsoleKeyInfo consoleKeyInfo;
        while ((consoleKeyInfo = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
        {
            if (!char.IsControl(consoleKeyInfo.KeyChar))
            {
                password.Add(consoleKeyInfo.KeyChar);
            }
            else if (consoleKeyInfo.Key == ConsoleKey.Backspace && password.Count > 0)
            {
                password.RemoveAt(password.Count - 1);
            }
        }
        Console.WriteLine();
        return password.ToArray();
    }
}