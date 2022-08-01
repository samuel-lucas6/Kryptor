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

namespace Kryptor;

public static class DisplayMessage
{
    public static void Error(string errorMessage)
    {
        Environment.ExitCode = Constants.ErrorCode;
        WriteLine($"Error: {errorMessage}", ConsoleColor.DarkRed);
    }

    public static void FilePathError(string filePath, string message)
    {
        Environment.ExitCode = Constants.ErrorCode;
        Error($"\"{Path.GetFileName(FileHandling.TrimTrailingSeparatorChars(filePath))}\" - {message}");
    }

    public static void Exception(string exceptionName, string errorMessage)
    {
        Environment.ExitCode = Constants.ErrorCode;
        WriteLine($"{exceptionName}: {errorMessage}", ConsoleColor.DarkRed);
    }

    public static void FilePathException(string filePath, string exceptionName, string errorMessage)
    {
        Environment.ExitCode = Constants.ErrorCode;
        WriteLine($"{exceptionName}: \"{Path.GetFileName(FileHandling.TrimTrailingSeparatorChars(filePath))}\" - {errorMessage}", ConsoleColor.DarkRed);
    }
    
    public static void KeyStringException(string keyString, string exceptionName, string errorMessage)
    {
        Environment.ExitCode = Constants.ErrorCode;
        WriteLine($"{exceptionName}: \"{keyString}\" - {errorMessage}", ConsoleColor.DarkRed);
    }
    
    public static void DerivingKeyFromPassword() => Console.WriteLine("Deriving encryption key from password...");
    
    public static void CreatingZipFile(string directoryPath, string zipFilePath)
    {
        Console.WriteLine($"Zipping \"{Path.GetFileName(directoryPath)}\" => \"{Path.GetFileName(zipFilePath)}\"...");
    }
    
    public static void ExtractingZipFile(string zipFilePath, string directoryPath)
    {
        Console.WriteLine($"Extracting \"{Path.GetFileName(zipFilePath)}\" => \"{Path.GetFileName(directoryPath)}\"...");
    }
    
    public static void EncryptingFile(string inputFilePath, string outputFilePath)
    {
        Console.WriteLine($"Encrypting \"{Path.GetFileName(inputFilePath)}\" => \"{Path.GetFileName(outputFilePath)}\"...");
    }

    public static void DecryptingFile(string inputFilePath, string outputFilePath)
    {
        Console.WriteLine($"Decrypting \"{Path.GetFileName(inputFilePath)}\" => \"{Path.GetFileName(outputFilePath)}\"...");
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

    public static void SymmetricKey(string symmetricKey)
    {
        Console.WriteLine($"Randomly generated key: {symmetricKey}");
        Console.WriteLine();
    }

    public static void Keyfile(string keyfilePath)
    {
        Console.WriteLine($"Randomly generated keyfile: \"{keyfilePath}\"");
        Console.WriteLine();
        WriteLine("IMPORTANT: Please back up this keyfile to external storage (e.g. memory sticks).", ConsoleColor.Blue);
        Console.WriteLine();
    }
    
    public static void KeyPair(string publicKey, string publicKeyFilePath, string privateKeyFilePath)
    {
        PublicKey(publicKey, publicKeyFilePath);
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
    
    public static void SigningFile(string filePath)
    {
        Console.WriteLine($"Signing \"{Path.GetFileName(filePath)}\"...");
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
        var errors = errorMessages.ToList();
        foreach (string errorMessage in errors)
        {
            Error(errorMessage);
        }
        return !errors.Any();
    }
}