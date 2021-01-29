using System;
using System.Security.Cryptography;

/*
    Kryptor: Free and open source file encryption.
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
    public static class ChunkHandling
    {
        private static readonly byte[] _keyCommitmentBlock = new byte[Constants.KeyCommitmentBlockLength];

        public static byte[] GetKeyCommitmentBlock()
        {
            return _keyCommitmentBlock;
        }

        public static byte[] PrependKeyCommitmentBlock(byte[] plaintextChunk)
        {
            return Utilities.ConcatArrays(_keyCommitmentBlock, plaintextChunk);
        }

        public static byte[] GetPreviousPoly1305Tag(byte[] ciphertextChunk)
        {
            byte[] previousTag = new byte[Constants.Poly1305Length];
            Array.Copy(ciphertextChunk, ciphertextChunk.Length - previousTag.Length, previousTag, destinationIndex: 0, previousTag.Length);
            return previousTag;
        }

        public static void ValidateKeyCommitmentBlock(byte[] plaintextChunk)
        {
            bool validKeyCommitmentBlock = CompareKeyCommitmentBlock(plaintextChunk);
            if (!validKeyCommitmentBlock) { throw new CryptographicException("Error decrypting message."); }
        }

        private static bool CompareKeyCommitmentBlock(byte[] plaintextChunk)
        {
            byte[] keyCommitmentBlock = new byte[Constants.KeyCommitmentBlockLength];
            Array.Copy(plaintextChunk, keyCommitmentBlock, keyCommitmentBlock.Length);
            return Sodium.Utilities.Compare(keyCommitmentBlock, _keyCommitmentBlock);
        }

        public static byte[] RemoveKeyCommitmentBlock(byte[] plaintextChunk)
        {
            byte[] plaintext = new byte[plaintextChunk.Length - Constants.KeyCommitmentBlockLength];
            Array.Copy(plaintextChunk, Constants.KeyCommitmentBlockLength, plaintext, destinationIndex: 0, plaintext.Length);
            return plaintext;
        }
    }
}
