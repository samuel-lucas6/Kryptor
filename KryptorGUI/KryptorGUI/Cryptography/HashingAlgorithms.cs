using Sodium;
using System.IO;

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
        public static byte[] Blake2(byte[] message, byte[] key)
        {
            return GenericHash.Hash(message, key, Constants.HashLength);
        }

        public static byte[] Blake2(FileStream fileStream, byte[] key)
        {
            using (var blake2 = new GenericHash.GenericHashAlgorithm(key, Constants.HashLength))
            {
                return blake2.ComputeHash(fileStream);
            }
        }

        public static byte[] Blake2(string message)
        {
            return GenericHash.Hash(message, (byte[])null, Constants.HashLength);
        }
    }
}
