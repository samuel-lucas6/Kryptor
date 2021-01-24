using System.Text;

/*
    Kryptor: Modern and secure file encryption.
    Copyright(C) 2020 Samuel Lucas

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
        public const string KryptorMagicBytes = "KRYPTOR";
        public static readonly int FileFormatVersion = 3;
        public const string EncryptedExtension = ".kryptor";
        public const string SaltFile = "Kryptor.salt";
        public const int KeyfileSize = 64;
        public const int FileBufferSize = 131072;
        public const int FileChunkSize = 16384;
        public const int KeyCommitmentBlockLength = 16;
        public const int BitConverterLength = 4;
        public const int EncryptedHeaderLength = 92;
        public static readonly int TotalChunkLength = KeyCommitmentBlockLength + BitConverterLength + FileChunkSize + Poly1305Length;

        // Cryptography
        public const int EncryptionKeySize = 32;
        public const int EphemeralPublicKeyLength = 32;
        public const int SaltLength = 16;
        public const int XChaChaNonceLength = 24;
        public const int Poly1305Length = 16;
        public const int BLAKE2Length = 64;
        public const string BLAKE2Personal = "Kryptor.Personal";

        // Digital signatures/asymmetric encryption
        public const int PublicKeyLength = 44;
        public const int PrivateKeyLength = 152;
        public const string PublicKeyExtension = ".public";
        public const string PrivateKeyExtension = ".private";
        public const int EncryptedPrivateKeyLength = 80;
        public const int SignatureLength = 64;
        public const string SignatureMagicBytes = "SIGNATURE";
        public const string SignatureExtension = ".signature";
    }
}
