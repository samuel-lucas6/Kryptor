using Konscious.Security.Cryptography;

/*  
    Kryptor: Free and open source file encryption software.
    Copyright(C) 2020 Samuel Lucas

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace Kryptor
{
    public static class KeyDerivation
    {
        public static (byte[], byte[]) DeriveKeys(byte[] passwordBytes, byte[] keyfileBytes, byte[] salt, byte[] associatedData, int parallelism, int memorySize, int iterations)
        {
            // 256-bit encryption key
            byte[] encryptionKey = new byte[Constants.EncryptionKeySize];
            // 1024-bit HMAC key
            byte[] hmacKey = new byte[Constants.HMACKeySize];
            MemoryEncryption.DecryptByteArray(ref passwordBytes);
            using (var argon2 = new Argon2id(passwordBytes))
            {
                argon2.DegreeOfParallelism = parallelism;
                argon2.MemorySize = memorySize;
                argon2.Iterations = iterations;
                argon2.Salt = salt;
                argon2.AssociatedData = associatedData;
                if (keyfileBytes != null)
                {
                    MemoryEncryption.DecryptByteArray(ref keyfileBytes);
                    argon2.KnownSecret = keyfileBytes;
                }
                encryptionKey = argon2.GetBytes(Constants.EncryptionKeySize);
                hmacKey = argon2.GetBytes(Constants.HMACKeySize);
            }
            MemoryEncryption.EncryptByteArray(ref passwordBytes);
            MemoryEncryption.EncryptByteArray(ref keyfileBytes);
            MemoryEncryption.EncryptByteArray(ref encryptionKey);
            MemoryEncryption.EncryptByteArray(ref hmacKey);
            return (encryptionKey, hmacKey);
        }
    }
}
