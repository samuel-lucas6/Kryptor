using System;

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
    public static class Generate
    {
        public static byte[] AssociatedData()
        {
            Enum cipher = (Cipher)Globals.EncryptionAlgorithm;
            string cipherName = Enum.GetName(cipher.GetType(), cipher);
            byte[] associatedData = HashingAlgorithms.Blake2(cipherName);
            return associatedData;
        }

        public static byte[] Salt()
        {
            return RandomNumberGenerator.GenerateRandomBytes(Constants.SaltLength);
        }

        public static byte[] Nonce()
        {
            if (Globals.EncryptionAlgorithm == (int)Cipher.XChaCha20 | Globals.EncryptionAlgorithm == (int)Cipher.XSalsa20)
            {
                return RandomNumberGenerator.GenerateRandomBytes(Constants.XChaChaNonceLength);
            }
            else if (Globals.EncryptionAlgorithm == (int)Cipher.AesCBC | Globals.EncryptionAlgorithm == (int)Cipher.AesCTR)
            {
                return RandomNumberGenerator.GenerateRandomBytes(Constants.AesNonceLength);
            }
            else
            {
                return null;
            }
        }
    }
}
