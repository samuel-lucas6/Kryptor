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
using System.Text;
using Geralt;

namespace Kryptor;

public static class Constants
{
    // Error handling
    public const int ErrorCode = -1;
    
    // Key derivation
    public const int HeaderKeySize = ChaCha20.KeySize + ChaCha20.NonceSize;
    public const int MemorySize = 268435456;
    public const int Iterations = 12;
    public static readonly byte[] Personalisation = Encoding.UTF8.GetBytes("Kryptor.Personal");
    
    // File encryption
    public static readonly byte[] EncryptionMagicBytes = Encoding.UTF8.GetBytes("KRYPTOR");
    public static readonly byte[] EncryptionVersion = { 0x04, 0x00 };
    public const string EncryptedExtension = ".kryptor";
    public const string KeyfileExtension = ".key";
    public const string ZipFileExtension = ".zip";
    public const int RandomFileNameLength = 16;
    public const int SymmetricKeyLength = 48;
    public static readonly byte[] SymmetricKeyHeader = Encoding.UTF8.GetBytes("Rn");
    public static readonly byte[] Base64Padding = Encoding.UTF8.GetBytes("=");
    public const int KeyfileLength = 64;
    public const int BoolBytesLength = 1;
    public const int LongBytesLength = 8;
    public const int FileNameHeaderLength = 255;
    public const int EncryptedHeaderLength = ChaCha20.KeySize + LongBytesLength + FileNameHeaderLength + LongBytesLength * 4 + BoolBytesLength + BLAKE2b.TagSize;
    public static readonly int FileHeadersLength = EncryptionMagicBytes.Length + EncryptionVersion.Length + X25519.PublicKeySize + Argon2id.SaltSize + EncryptedHeaderLength;
    public static readonly int UnencryptedHeadersLength = FileHeadersLength - EncryptedHeaderLength;
    public const int FileChunkSize = 16384;
    public const int CiphertextChunkLength = FileChunkSize + BLAKE2b.TagSize;

    // Asymmetric keys
    public static readonly string DefaultKeyDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), EncryptedExtension);
    public const string DefaultEncryptionKeyFileName = "encryption";
    public const string DefaultSigningKeyFileName = "signing";
    public const string PublicKeyExtension = ".public";
    public const string PrivateKeyExtension = ".private";
    public static readonly string DefaultEncryptionPublicKeyPath = Path.Combine(DefaultKeyDirectory, DefaultEncryptionKeyFileName + PublicKeyExtension);
    public static readonly string DefaultEncryptionPrivateKeyPath = Path.Combine(DefaultKeyDirectory, DefaultEncryptionKeyFileName + PrivateKeyExtension);
    public static readonly string DefaultSigningPublicKeyPath = Path.Combine(DefaultKeyDirectory, DefaultSigningKeyFileName + PublicKeyExtension);
    public static readonly string DefaultSigningPrivateKeyPath = Path.Combine(DefaultKeyDirectory, DefaultSigningKeyFileName + PrivateKeyExtension);
    public const int PublicKeyLength = 48;
    public const int V2EncryptionPrivateKeyLength = 112;
    public const int V1EncryptionPrivateKeyLength = 144;
    public const int V2SigningPrivateKeyLength = 156;
    public const int V1SigningPrivateKeyLength = 188;
    public const int KeyAlgorithmLength = 2;
    public static readonly byte[] Curve25519KeyHeader = Encoding.UTF8.GetBytes("Cu");
    public static readonly byte[] Ed25519KeyHeader = Encoding.UTF8.GetBytes("Ed");
    public static readonly byte[] PrivateKeyVersion1 = { 0x01, 0x00 };
    public static readonly byte[] PrivateKeyVersion2 = { 0x02, 0x00 };
    
    // File signing
    public const string SignatureExtension = ".signature";
    public static readonly byte[] SignatureMagicBytes = Encoding.UTF8.GetBytes("SIGNATURE");
    public static readonly byte[] SignatureVersion = { 0x01, 0x00 };
}