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
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace KryptorCLI;

public static class DisplayMessage
{
    private const string ErrorWord = "Error";
    
    public static void Error(string errorMessage) => WriteLine($"{ErrorWord}: {errorMessage}", ConsoleColor.DarkRed);

    public static void FilePathMessage(string filePath, string message) => WriteLine($"\"{Path.GetFileName(filePath)}\": {message}", ConsoleColor.DarkRed);

    public static void FilePathError(string filePath, string errorMessage) => WriteLine($"\"{Path.GetFileName(filePath)}\" - {ErrorWord}: {errorMessage}", ConsoleColor.DarkRed);
    
    public static void Exception(string exceptionName, string errorMessage) => WriteLine($"{ErrorWord}: {exceptionName} - {errorMessage}", ConsoleColor.DarkRed);
    
    public static void FilePathException(string filePath, string exceptionName, string errorMessage)
    {
        WriteLine($"\"{Path.GetFileName(filePath)}\" - {ErrorWord}: {exceptionName} - {errorMessage}", ConsoleColor.DarkRed);
    }

    public static void DerivingKeyFromPassword() => Console.WriteLine("Deriving encryption key from password...");

    public static void EncryptingFile(string inputFilePath, string outputFilePath)
    {
        Console.WriteLine($"Encrypting \"{Path.GetFileName(inputFilePath)}\" => \"{Path.GetFileName(outputFilePath)}\"...");
    }

    public static void DecryptingFile(string inputFilePath, string outputFilePath)
    {
        Console.WriteLine($"Decrypting \"{Path.GetFileName(inputFilePath)}\" => \"{Path.GetFileName(outputFilePath)}\"...");
    }

    public static void DirectoryEncryptionComplete(string directoryPath)
    {
        Console.WriteLine();
        WriteLine($"Encryption of \"{Path.GetFileName(directoryPath)}\" directory complete.", ConsoleColor.Blue);
    }

    public static void DirectoryDecryptionComplete(string directoryPath)
    {
        Console.WriteLine();
        WriteLine($"Decryption of \"{Path.GetFileName(directoryPath)}\" directory complete.", ConsoleColor.Blue);
    }

    public static void SuccessfullyEncrypted(bool space = true)
    {
        if (Globals.TotalCount <= 0) { return; }
        if (space) { Console.WriteLine(); }
        WriteLine($"Successfully encrypted: {Globals.SuccessfulCount}/{Globals.TotalCount}", Globals.SuccessfulCount == Globals.TotalCount ? ConsoleColor.Green : ConsoleColor.DarkRed);
    }

    public static void SuccessfullyDecrypted(bool space = true)
    {
        if (Globals.TotalCount <= 0) { return; }
        if (space) { Console.WriteLine(); }
        WriteLine($"Successfully decrypted: {Globals.SuccessfulCount}/{Globals.TotalCount}", Globals.SuccessfulCount == Globals.TotalCount ? ConsoleColor.Green : ConsoleColor.DarkRed);
    }
    
    public static void KeyPair(string publicKey, string publicKeyFilePath, string privateKeyFilePath)
    {
        Console.WriteLine();
        Console.WriteLine($"Public key: {publicKey}");
        Console.WriteLine($"Public key file: \"{publicKeyFilePath}\"");
        Console.WriteLine();
        Console.Write($"Private key file: \"{privateKeyFilePath}\" - ");
        WriteLine("Keep this secret!", ConsoleColor.DarkRed);
        Console.WriteLine();
        WriteLine("IMPORTANT: Please back up these files to external storage (e.g. memory sticks).", ConsoleColor.Blue);
    }
    
    public static void PublicKey(string publicKey, string publicKeyFilePath)
    {
        Console.WriteLine();
        Console.WriteLine($"Public key: {publicKey}");
        if (!string.IsNullOrEmpty(publicKeyFilePath)) { Console.WriteLine($"Public key file: \"{publicKeyFilePath}\""); }
    }

    public static void SuccessfullySigned()
    {
        if (Globals.TotalCount <= 0) { return; }
        Console.WriteLine();
        WriteLine($"Successfully signed: {Globals.SuccessfulCount}/{Globals.TotalCount}", Globals.SuccessfulCount == Globals.TotalCount ? ConsoleColor.Green : ConsoleColor.DarkRed);
    }
    
    public static void WriteLine(string message, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static bool AnyErrors(IEnumerable<string> errorMessages)
    {
        foreach (string errorMessage in errorMessages)
        {
            Error(errorMessage);
        }
        return !errorMessages.Any();
    }
}