using System;
using System.Collections.Generic;

/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright(C) 2020-2021 Samuel Lucas

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
    public static class PasswordPrompt
    {
        public static char[] EnterNewPassword()
        {
            Console.WriteLine("Enter a password (empty for random passphrase):");
            char[] password = GetPassword();
            if (password.Length == 0)
            {
                const int length = 8;
                password = PassphraseGenerator.GetRandomPassphrase(length);
                DisplayRandomPassphrase(password);
            }
            else
            {
                RetypeNewPassword(password);
            }
            return password;
        }

        private static void DisplayRandomPassphrase(char[] password)
        {
            Console.Write($"Randomly generated passphrase: ");
            foreach (char character in password)
            {
                Console.Write(character);
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void RetypeNewPassword(char[] password)
        {
            Console.WriteLine("Retype password:");
            char[] retypedPassword = GetPassword();
            if (!Utilities.Compare(password, retypedPassword))
            {
                DisplayMessage.Error("Passwords do not match.");
                Environment.Exit(13);
            }
        }

        public static char[] EnterYourPassword()
        {
            char[] password;
            do
            {
                Console.WriteLine("Enter your password:");
                password = GetPassword();
            } while (password.Length == 0);
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
}
