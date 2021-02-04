using Sodium;

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
    public static class Generate
    {
        public static byte[] KeyEncryptionKey(byte[] sharedSecret, byte[] ephemeralSharedSecret, byte[] salt)
        {
            byte[] inputKeyingMaterial = Utilities.ConcatArrays(sharedSecret, ephemeralSharedSecret);
            byte[] keyEncryptionKey = Blake2.KeyDerivation(inputKeyingMaterial, salt, Constants.EncryptionKeyLength);
            Utilities.ZeroArray(ephemeralSharedSecret);
            Utilities.ZeroArray(inputKeyingMaterial);
            return keyEncryptionKey;
        }

        public static byte[] KeyEncryptionKey(byte[] ephemeralSharedSecret, byte[] salt)
        {
            byte[] keyEncryptionKey = Blake2.KeyDerivation(ephemeralSharedSecret, salt, Constants.EncryptionKeyLength);
            Utilities.ZeroArray(ephemeralSharedSecret);
            return keyEncryptionKey;
        }

        public static byte[] RandomSalt()
        {
            return SodiumCore.GetRandomBytes(Constants.SaltLength);
        }

        public static byte[] RandomEphemeralPublicKeyHeader()
        {
            using var keyPair = PublicKeyBox.GenerateKeyPair();
            return keyPair.PublicKey;
        }

        public static byte[] RandomDataEncryptionKey()
        {
            return SodiumCore.GetRandomBytes(Constants.EncryptionKeyLength);
        }

        public static byte[] RandomNonce()
        {
            return SodiumCore.GetRandomBytes(Constants.XChaChaNonceLength);
        }

        public static byte[] RandomKeyfile()
        {
            return SodiumCore.GetRandomBytes(Constants.KeyfileLength);
        }
    }
}
