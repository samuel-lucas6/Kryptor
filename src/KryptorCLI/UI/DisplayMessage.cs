using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2021 Samuel Lucas

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
    public static class DisplayMessage
    {
        private const string _error = "Error";

        public static void FilePathException(string filePath, string exceptionName, string errorMessage)
        {
            Console.WriteLine($"{Path.GetFileName(filePath)} - {_error}: {exceptionName} - {errorMessage}");
        }

        public static void Exception(string exceptionName, string errorMessage)
        {
            Console.WriteLine($"{_error}: {exceptionName} - {errorMessage}");
        }

        public static void Error(string errorMessage)
        {
            Console.WriteLine($"{_error}: {errorMessage}");
        }

        public static void FilePathMessage(string filePath, string message)
        {
            Console.WriteLine($"{Path.GetFileName(filePath)}: {message}");
        }

        public static void FilePathError(string filePath, string message)
        {
            Console.WriteLine($"{Path.GetFileName(filePath)} - {_error}: {message}");
        }

        public static void EncryptingFile(string inputFilePath, string outputFilePath)
        {
            Console.WriteLine($"Encrypting {Path.GetFileName(inputFilePath)} => {Path.GetFileName(outputFilePath)}...");
        }

        public static void DecryptingFile(string inputFilePath, string outputFilePath)
        {
            Console.WriteLine($"Decrypting {Path.GetFileName(inputFilePath)} => {Path.GetFileName(outputFilePath)}...");
        }

        public static void DirectoryEncryptionComplete(string directoryPath)
        {
            Console.WriteLine($"Encryption of {Path.GetFileName(directoryPath)} directory completed.");
        }

        public static void DirectoryDecryptionComplete(string directoryPath)
        {
            Console.WriteLine($"Decryption of {Path.GetFileName(directoryPath)} directory completed.");
        }

        public static void SuccessfullyEncrypted()
        {
            if (Globals.TotalCount > 0)
            {
                Console.WriteLine();
                Console.WriteLine($"Successfully encrypted: {Globals.SuccessfulCount}/{Globals.TotalCount}");
            }
        }

        public static void SuccessfullyDecrypted()
        {
            if (Globals.TotalCount > 0)
            {
                Console.WriteLine();
                Console.WriteLine($"Successfully decrypted: {Globals.SuccessfulCount}/{Globals.TotalCount}");
            }
        }

        public static void SuccessfullySigned()
        {
            if (Globals.TotalCount > 0)
            {
                Console.WriteLine();
                Console.WriteLine($"Successfully signed: {Globals.SuccessfulCount}/{Globals.TotalCount}");
            }
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
}
