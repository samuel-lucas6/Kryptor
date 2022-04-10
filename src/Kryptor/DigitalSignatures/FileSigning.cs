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

    public static void SignEachFile(byte[] privateKey, string comment, bool prehash, string signatureFilePath, string[] filePaths)
    {
        if (privateKey == null || filePaths == null) { return; }
        if (string.IsNullOrEmpty(comment)) { comment = DefaultComment; }
        foreach (string filePath in filePaths)
        {
            try
            {
                Console.WriteLine();
                if (FileHandling.IsDirectory(filePath))
                {
                    Console.WriteLine($"Signing each file in \"{Path.GetFileName(filePath)}\" directory...");
                    string[] files = FileHandling.GetAllFiles(filePath);
                    Globals.TotalCount += files.Length - 1;
                    foreach (string file in files)
                    {
                        try
                        {
                            DisplayMessage.SigningFile(file);
                            DigitalSignatures.SignFile(file, signatureFilePath: string.Empty, comment, prehash, privateKey);
                        }
                        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
                        {
                            DisplayMessage.FilePathException(filePath, ex.GetType().Name, "Unable to sign the file.");
                        }
                    }
                    continue;
                }
                DisplayMessage.SigningFile(filePath);
                DigitalSignatures.SignFile(filePath, signatureFilePath, comment, prehash, privateKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(filePath, ex.GetType().Name, !Directory.Exists(filePath) ? "Unable to sign the file." : "Unable to sign the files in the directory.");
            }
        }
        CryptographicOperations.ZeroMemory(privateKey);
        DisplayMessage.SuccessfullySigned();
    }
       
    public static void VerifyFile(string signatureFilePath, string filePath, byte[] publicKey)
    {
        if (signatureFilePath == null || filePath == null || publicKey == null) { return; }
        try
        {
            Console.WriteLine($"Verifying \"{Path.GetFileName(signatureFilePath)}\"...");
            bool validSignature = DigitalSignatures.VerifySignature(signatureFilePath, filePath, publicKey, out string comment);
            Console.WriteLine();
            if (!validSignature)
            {
                DisplayMessage.WriteLine("Bad signature.", ConsoleColor.DarkRed);
                return;
            }
            DisplayMessage.WriteLine("Good signature.", ConsoleColor.Green);
            if (!string.IsNullOrWhiteSpace(comment)) { Console.WriteLine($"Authenticated comment: {comment}"); }
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            if (ex is ArgumentException)
            {
                DisplayMessage.FilePathMessage(signatureFilePath, ex.Message);
                return;
            }
            DisplayMessage.Exception(ex.GetType().Name, "Unable to verify the signature.");
        }
    }
}