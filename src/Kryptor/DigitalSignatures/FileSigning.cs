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
using System.Security.Cryptography;

namespace Kryptor;

public static class FileSigning
{
    private const string DefaultComment = "This file has not been tampered with.";

    public static void SignEachFile(string[] filePaths, string[] signaturePaths, string comment, bool prehash, Span<byte> privateKey)
    {
        if (filePaths == null || privateKey.Length == 0) {
            throw new UserInputException();
        }
        signaturePaths ??= new string[filePaths.Length];
        if (string.IsNullOrEmpty(comment)) {
            comment = DefaultComment;
        }
        for (int i = 0; i < filePaths.Length; i++) {
            try
            {
                if (File.GetAttributes(filePaths[i]).HasFlag(FileAttributes.Directory)) {
                    SignDirectoryFiles(filePaths[i], comment, prehash, privateKey);
                    continue;
                }
                if (string.IsNullOrEmpty(signaturePaths[i])) {
                    signaturePaths[i] = filePaths[i] + Constants.SignatureExtension;
                }
                DisplayMessage.InputToOutput("Signing", filePaths[i], signaturePaths[i]);
                DigitalSignatures.SignFile(filePaths[i], signaturePaths[i], comment, prehash, privateKey);
                Globals.SuccessfulCount++;
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(filePaths[i], ex.GetType().Name, !Directory.Exists(filePaths[i]) ? "Unable to sign the file." : "Unable to sign the files in the directory.");
            }
            Console.WriteLine();
        }
        CryptographicOperations.ZeroMemory(privateKey);
        DisplayMessage.SuccessfullySigned();
    }

    private static void SignDirectoryFiles(string directoryPath, string comment, bool prehash, Span<byte> privateKey)
    {
        Console.WriteLine($"Signing each file in \"{Path.GetFileName(directoryPath)}\" directory...");
        string[] filePaths = Directory.GetFiles(directoryPath, searchPattern: "*", SearchOption.AllDirectories);
        string[] signaturePaths = new string[filePaths.Length];
        Globals.TotalCount += filePaths.Length - 1;
        for (int i = 0; i < filePaths.Length; i++) {
            try
            {
                signaturePaths[i] = filePaths[i] + Constants.SignatureExtension;
                DisplayMessage.InputToOutput("Signing", filePaths[i], signaturePaths[i]);
                DigitalSignatures.SignFile(filePaths[i], signaturePaths[i], comment, prehash, privateKey);
                Globals.SuccessfulCount++;
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(filePaths[i], ex.GetType().Name, "Unable to sign the file.");
            }
        }
    }
    
    public static void VerifyEachFile(string[] signaturePaths, string[] filePaths, Span<byte> publicKey)
    {
        if (filePaths == null || publicKey.Length == 0) {
            throw new UserInputException();
        }
        signaturePaths ??= new string[filePaths.Length];
        for (int i = 0; i < filePaths.Length; i++) {
            try
            {
                if (string.IsNullOrEmpty(signaturePaths[i])) {
                    signaturePaths[i] = filePaths[i] + Constants.SignatureExtension;
                }
                Console.WriteLine($"Verifying \"{Path.GetFileName(signaturePaths[i])}\"...");
                bool validSignature = DigitalSignatures.VerifySignature(signaturePaths[i], filePaths[i], publicKey, out string comment);
                if (!validSignature) {
                    DisplayMessage.WriteLine("Bad signature.", ConsoleColor.DarkRed);
                }
                else {
                    DisplayMessage.WriteLine("Good signature.", ConsoleColor.Green);
                    if (!string.IsNullOrWhiteSpace(comment)) {
                        Console.WriteLine($"Authenticated comment: {comment}");
                    }
                }
                if (i != filePaths.Length - 1) {
                    Console.WriteLine();
                }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(signaturePaths[i], ex.GetType().Name, "Unable to verify the signature.");
            }
        }
    }
}