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

using System.IO;

namespace KryptorCLI;

public static class ErrorMessages
{
    public const string InvalidPrivateKeyFile = "Please specify a valid private key file.";
    public const string NonExistentDefaultPrivateKeyFile = "You don't have a default key pair. You can generate one using -g|--generate.";
    public const string NonExistentPrivateKeyFile = "Please specify a private key file that exists.";
    public const string InvalidPublicKey = "Please specify a valid public key.";
    public const string InvalidPublicKeyFile = "Please specify a valid public key file.";
    public const string NonExistentPublicKeyFile = "Please specify a public key file that exists.";
    public const string NoFileToVerify = "Please specify a file to verify.";
    public const string NoFileToSign = "Please specify a file to sign.";
    public const string PasswordBasedEncryption = "Please specify a password and/or keyfile, private key, or private key and public key.";
    public const string UnableToEncryptFile = "Unable to encrypt the file.";
    public const string UnableToDecryptFile = "Unable to decrypt the file.";
    
    public static string GetFilePathError(string filePath, string errorMessage) => $"\"{Path.GetFileName(filePath)}\" - {errorMessage}";
}