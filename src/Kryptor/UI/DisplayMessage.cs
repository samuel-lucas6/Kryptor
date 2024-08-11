/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2023 Samuel Lucas

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
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Kryptor;

public static class DisplayMessage
{
    public static void Error(string message)
    {
        Environment.ExitCode = Constants.ErrorCode;
        ErrorWriteLine($"Error: {message}", ConsoleColor.DarkRed);
    }

    public static void FilePathError(string filePath, string message)
    {
        Error($"\"{Path.GetFileName(FileHandling.TrimTrailingSeparatorChars(filePath))}\" - {message}");
    }

    public static void Exception(string exceptionName, string message)
    {
        Environment.ExitCode = Constants.ErrorCode;
        ErrorWriteLine($"{exceptionName}: {message}", ConsoleColor.DarkRed);
    }

    public static void FilePathException(string filePath, string exceptionName, string message)
    {
        Environment.ExitCode = Constants.ErrorCode;
        ErrorWriteLine($"{exceptionName}: \"{Path.GetFileName(FileHandling.TrimTrailingSeparatorChars(filePath))}\" - {message}", ConsoleColor.DarkRed);
    }

    public static void DerivingKeyFromPassphrase() => Console.WriteLine("Deriving encryption key from passphrase...");

    public static void InputToOutput(string activity, string inputFilePath, string outputFilePath)
    {
        Console.WriteLine($"{activity} \"{Path.GetFileName(inputFilePath)}\" => \"{Path.GetFileName(outputFilePath)}\"...");
    }

    public static void SuccessfullyEncrypted()
    {
        WriteLine($"Successfully encrypted: {Globals.SuccessfulCount}/{Globals.TotalCount}", Globals.SuccessfulCount == Globals.TotalCount ? ConsoleColor.Green : ConsoleColor.DarkRed);
    }

    public static void SuccessfullyDecrypted()
    {
        WriteLine($"Successfully decrypted: {Globals.SuccessfulCount}/{Globals.TotalCount}", Globals.SuccessfulCount == Globals.TotalCount ? ConsoleColor.Green : ConsoleColor.DarkRed);
    }

    public static void SuccessfullySigned()
    {
        WriteLine($"Successfully signed: {Globals.SuccessfulCount}/{Globals.TotalCount}", Globals.SuccessfulCount == Globals.TotalCount ? ConsoleColor.Green : ConsoleColor.DarkRed);
    }

    public static void SymmetricKey(string symmetricKey)
    {
        Console.WriteLine($"Randomly generated key: {symmetricKey}");
        Console.WriteLine();
    }

    public static void Keyfile(string keyfilePath)
    {
        Console.WriteLine($"Randomly generated keyfile: \"{keyfilePath}\"");
        Console.WriteLine();
        WriteLine("IMPORTANT: Please back up this keyfile to external storage (e.g. memory sticks).", ConsoleColor.DarkYellow);
        Console.WriteLine();
    }

    public static void KeyPair(string publicKey, string publicKeyPath, string privateKeyPath)
    {
        Console.WriteLine();
        PublicKey(publicKey, publicKeyPath);
        Console.WriteLine();
        Console.Write($"Private key file: \"{privateKeyPath}\" - ");
        WriteLine("Keep this secret!", ConsoleColor.DarkYellow);
        Console.WriteLine();
        WriteLine("IMPORTANT: Please back up these files to external storage (e.g. memory sticks).", ConsoleColor.DarkYellow);
    }

    public static void PublicKey(string publicKey, string publicKeyPath)
    {
        Console.WriteLine($"Public key: {publicKey}");
        if (!string.IsNullOrEmpty(publicKeyPath)) {
            Console.WriteLine($"Public key file: \"{publicKeyPath}\"");
        }
    }

    public static void WriteLine(string message, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void ErrorWriteLine(string message, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.Error.WriteLine(message);
        Console.ResetColor();
    }

    public static void AllErrors(IEnumerable<string> errorMessages)
    {
        var errors = errorMessages.ToList();
        foreach (string errorMessage in errors) {
            Error(errorMessage);
        }
        if (errors.Count != 0) {
            throw new UserInputException($"{errors.Count} errors detected during initial validation.");
        }
    }
}
