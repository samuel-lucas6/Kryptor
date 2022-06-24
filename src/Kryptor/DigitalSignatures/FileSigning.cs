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

    public static void SignEachFile(byte[] privateKey, string comment, bool prehash, string[] signatureFilePaths, string[] filePaths)
    {
        if (privateKey == null || filePaths == null) { return; }
        if (string.IsNullOrEmpty(comment)) { comment = DefaultComment; }
        for (int i = 0; i < filePaths.Length; i++)
        {
            try
            {
                Console.WriteLine();
                if (FileHandling.IsDirectory(filePaths[i]))
                {
                    Console.WriteLine($"Signing each file in \"{Path.GetFileName(filePaths[i])}\" directory...");
                    string[] files = FileHandling.GetAllFiles(filePaths[i]);
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
                            DisplayMessage.FilePathException(file, ex.GetType().Name, "Unable to sign the file.");
                        }
                    }
                    continue;
                }
                DisplayMessage.SigningFile(filePaths[i]);
                DigitalSignatures.SignFile(filePaths[i], signatureFilePaths == null ? string.Empty : signatureFilePaths[i], comment, prehash, privateKey);
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(filePaths[i], ex.GetType().Name, !Directory.Exists(filePaths[i]) ? "Unable to sign the file." : "Unable to sign the files in the directory.");
            }
        }
        CryptographicOperations.ZeroMemory(privateKey);
        DisplayMessage.SuccessfullySigned();
    }
       
    public static void VerifyEachFile(string[] signatureFilePaths, string[] filePaths, byte[] publicKey)
    {
        if (filePaths == null || publicKey == null) { return; }
        signatureFilePaths ??= new string[filePaths.Length];
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (string.IsNullOrEmpty(signatureFilePaths[i])) { signatureFilePaths[i] = filePaths[i] + Constants.SignatureExtension; }
            try
            {
                Console.WriteLine($"Verifying \"{Path.GetFileName(signatureFilePaths[i])}\"...");
                bool validSignature = DigitalSignatures.VerifySignature(signatureFilePaths[i], filePaths[i], publicKey, out string comment);
                if (!validSignature) { DisplayMessage.WriteLine("Bad signature.", ConsoleColor.DarkRed); }
                else
                {
                    DisplayMessage.WriteLine("Good signature.", ConsoleColor.Green);
                    if (!string.IsNullOrWhiteSpace(comment)) { Console.WriteLine($"Authenticated comment: {comment}"); }
                }
                if (i != filePaths.Length - 1) { Console.WriteLine(); }
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(signatureFilePaths[i], ex.GetType().Name, "Unable to verify the signature.");
            }
        }
    }
}