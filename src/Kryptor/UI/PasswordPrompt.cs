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
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Geralt;

namespace Kryptor;

public static class PasswordPrompt
{
    public static Span<byte> GetNewPassword(Span<byte> password)
    {
        return password.Length switch
        {
            0 => EnterNewPassword(),
            1 when ConstantTime.Equals(password, Encoding.UTF8.GetBytes(" ")) => UseRandomPassphrase(),
            _ => password
        };
    }
    
    private static Span<byte> EnterNewPassword()
    {
        Console.WriteLine("Enter a password (leave empty for a random passphrase):");
        Span<byte> password = GetPassword();
        if (password.Length == 0) {
            return UseRandomPassphrase();
        }
        RetypeNewPassword(password);
        return password;
    }

    private static void RetypeNewPassword(Span<byte> password)
    {
        Console.WriteLine("Retype password:");
        Span<byte> retypedPassword = GetPassword();
        if (retypedPassword.Length == 0 || !ConstantTime.Equals(password, retypedPassword)) {
            DisplayMessage.Error("The passwords don't match.");
            Environment.Exit(Constants.ErrorCode);
        }
        CryptographicOperations.ZeroMemory(retypedPassword);
    }
    
    private static Span<byte> UseRandomPassphrase()
    {
        char[] passphrase = SecureRandom.GetPassphrase(wordCount: 8);
        Console.Write("Randomly generated passphrase: ");
        Console.WriteLine(passphrase);
        Console.WriteLine();
        return Encoding.UTF8.GetBytes(passphrase);
    }

    public static Span<byte> EnterYourPassword(bool isPrivateKey = false)
    {
        Console.WriteLine(isPrivateKey == false ? "Enter your password:" : "Enter your private key password:");
        Span<byte> password = GetPassword();
        if (password.Length == 0) {
            DisplayMessage.Error("You didn't enter a password.");
            Environment.Exit(Constants.ErrorCode);
        }
        return password;
    }

    private static Span<byte> GetPassword()
    {
        var password = new List<char>();
        ConsoleKeyInfo consoleKeyInfo;
        while ((consoleKeyInfo = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter) {
            if (!char.IsControl(consoleKeyInfo.KeyChar)) {
                password.Add(consoleKeyInfo.KeyChar);
            }
            else if (consoleKeyInfo.Key == ConsoleKey.Backspace && password.Count > 0) {
                password.RemoveAt(password.Count - 1);
            }
        }
        Console.WriteLine();
        return Encoding.UTF8.GetBytes(password.ToArray());
    }
}