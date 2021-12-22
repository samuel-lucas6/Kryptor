/*
    Kryptor: A simple, modern, and secure encryption tool.
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

namespace KryptorCLI;

public static class FileSigning
{
    private const string DefaultComment = "This file has not been tampered with.";

    public static void SignEachFile(string[] filePaths, string signatureFilePath, string comment, bool preHash, byte[] privateKey)
    {
        privateKey = PrivateKey.Decrypt(privateKey);
        if (privateKey == null) { return; }
        if (string.IsNullOrEmpty(comment)) { comment = DefaultComment; }
        foreach (string filePath in filePaths)
        {
            try
            {
                Console.WriteLine($"Signing {Path.GetFileName(filePath)}...");
                DigitalSignatures.SignFile(filePath, signatureFilePath, comment, preHash, privateKey);
                Globals.SuccessfulCount += 1;
            }
            catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
            {
                DisplayMessage.FilePathException(filePath, ex.GetType().Name, "Unable to create signature.");
            }
        }
        CryptographicOperations.ZeroMemory(privateKey);
        DisplayMessage.SuccessfullySigned();
    }
       
    public static void VerifyFile(string signatureFilePath, string filePath, byte[] publicKey)
    {
        try
        {
            Console.WriteLine($"Verifying {Path.GetFileName(signatureFilePath)}...");
            bool validSignature = DigitalSignatures.VerifySignature(signatureFilePath, filePath, publicKey, out string comment);
            Console.WriteLine();
            if (!validSignature)
            {
                Console.WriteLine("Bad signature.");
                return;
            }
            Console.WriteLine("Good signature.");
            Console.WriteLine($"Authenticated comment: {comment}");
        }
        catch (Exception ex) when (ExceptionFilters.Cryptography(ex))
        {
            if (ex is ArgumentException)
            {
                DisplayMessage.FilePathMessage(signatureFilePath, ex.Message);
                return;
            }
            DisplayMessage.Exception(ex.GetType().Name, "Unable to verify signature.");
        }
    }
}