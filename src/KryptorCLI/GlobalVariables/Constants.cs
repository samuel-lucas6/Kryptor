using System;
using System.IO;
using System.Text;

/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright(C) 2020-2021 Samuel Lucas

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
    public static class Constants
    {
        // Key derivation
        public const int Mebibyte = 1048576;
        public static readonly int MemorySize = 256 * Mebibyte;
        public static readonly int Iterations = 12;

        // File encryption
        public static readonly byte[] KryptorMagicBytes = Encoding.UTF8.GetBytes("KRYPTOR");
        public static readonly byte[] EncryptionVersion = BitConversion.GetBytes((short)3);
        public const string EncryptedExtension = ".kryptor";
        public const string SaltFile = "Kryptor.salt";
        public const int KeyfileLength = 64;
        public const int FileStreamBufferSize = 131072;
        public const int FileChunkSize = 16384;
        public const int KeyCommitmentBlockLength = 16;
        public const int IntBitConverterLength = 4;
        public const int EncryptedHeaderLength = 72;
        public static readonly int TotalChunkLength = KeyCommitmentBlockLength + FileChunkSize + Poly1305Length;

        // Cryptography
        public const int EncryptionKeyLength = 32;
        public const int EphemeralPublicKeyLength = 32;
        public const int SaltLength = 16;
        public const int XChaChaNonceLength = 24;
        public const int Poly1305Length = 16;
        public const int BLAKE2Length = 64;
        public const string BLAKE2Personal = "Kryptor.Personal";

        // Asymmetric keys
        public static readonly string DefaultKeyDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), EncryptedExtension);
        public static readonly string DefaultEncryptionKeyFileName = "encryption";
        public static readonly string DefaultSigningKeyFileName = "signing";
        public const string PublicKeyExtension = ".public";
        public const string PrivateKeyExtension = ".private";
        public static readonly string DefaultEncryptionPublicKeyPath = Path.Combine(DefaultKeyDirectory, DefaultEncryptionKeyFileName + PublicKeyExtension);
        public static readonly string DefaultEncryptionPrivateKeyPath = Path.Combine(DefaultKeyDirectory, DefaultEncryptionKeyFileName + PrivateKeyExtension);
        public static readonly string DefaultSigningPublicKeyPath = Path.Combine(DefaultKeyDirectory, DefaultSigningKeyFileName + PublicKeyExtension);
        public static readonly string DefaultSigningPrivateKeyPath = Path.Combine(DefaultKeyDirectory, DefaultSigningKeyFileName + PrivateKeyExtension);
        public const int PublicKeyLength = 48;
        public const int EncryptionPrivateKeyLength = 144;
        public const int SigningPrivateKeyLength = 188;
        public static readonly byte[] Curve25519KeyHeader = Encoding.UTF8.GetBytes("Cu");
        public static readonly byte[] Ed25519KeyHeader = Encoding.UTF8.GetBytes("Ed");
        public static readonly byte[] PrivateKeyVersion = BitConversion.GetBytes((short)1);

        // File signing
        public const string SignatureExtension = ".signature";
        public static readonly byte[] SignatureMagicBytes = Encoding.UTF8.GetBytes("SIGNATURE");
        public static readonly byte[] SignatureVersion = BitConversion.GetBytes((short)1);
        public const int SignatureLength = 64;
    }
}
