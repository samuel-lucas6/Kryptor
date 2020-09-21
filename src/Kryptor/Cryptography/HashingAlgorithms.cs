using Konscious.Security.Cryptography;
using Sodium;
using System.IO;
using System.Security.Cryptography;

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
    public static class HashingAlgorithms
    {
        public static byte[] Blake2(byte[] message)
        {
            using (var blake2 = new HMACBlake2B(512))
            {
                message = blake2.ComputeHash(message);
                return message;
            }
        }

        public static byte[] Blake2(string message)
        {
            byte[] hash = GenericHash.Hash(message, (byte[]) null, 64);
            return hash;
        }

        public static byte[] HMAC(FileStream fileStream, byte[] key)
        {
            using (var hmac = new HMACSHA512(key))
            {
                byte[] hash = hmac.ComputeHash(fileStream);
                return hash;
            }
        }

        public static byte[] HMAC(byte[] message, byte[] key)
        {
            using (var hmac = new HMACSHA512(key))
            {
                message = hmac.ComputeHash(message);
                return message;
            }
        }
    }
}
