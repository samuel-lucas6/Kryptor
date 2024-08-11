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

using System.IO;

namespace Kryptor;

public static class ErrorMessages
{
    public const string FileOrDirectoryDoesNotExist = "This file/directory doesn't exist.";
    public const string DirectoryDoesNotExist = "This directory doesn't exist.";
    public const string DirectoryEmpty = "This directory is empty.";
    public const string UnableToAccessDirectory = "Unable to access this directory.";
    public const string InvalidPrivateKeyFile = "This isn't a .private key file.";
    public const string NonExistentDefaultPrivateKeyFile = "You don't have a default key pair. You can generate one using -g|--generate.";
    public const string NonExistentPrivateKeyFile = "This private key file doesn't exist.";
    public const string NoPublicKey = "No public key specified.";
    public const string MultiplePublicKeys = "Specify a single public key.";
    public const string InvalidPublicKey = "Invalid public key.";
    public const string InvalidPublicKeyFile = "This isn't a .public key file.";
    public const string NonExistentPublicKeyFile = "This public key file doesn't exist.";
    public const string InvalidPublicKeyFileLength = "Invalid public key file length.";
    public const string InvalidPrivateKeyFileLength = "Invalid private key file length.";
    public static readonly string InvalidCommentLength = $"The max comment length is {Constants.MaxCommentLength} characters.";
    public const string EncryptionMethod = "Specify a passphrase and/or symmetric key, private key, or private key and public key.";
    public const string InvalidSymmetricKey = "Invalid symmetric key string.";
    public const string UnableToEncryptFile = "Unable to encrypt the file/directory.";
    public const string UnableToDecryptFile = "Unable to decrypt the file.";

    public static string GetFilePathError(string filePath, string errorMessage) => $"\"{Path.GetFileName(FileHandling.TrimTrailingSeparatorChars(filePath))}\" - {errorMessage}";

    public static string GetKeyStringError(string keyString, string errorMessage) => $"\"{keyString}\" - {errorMessage}";
}
