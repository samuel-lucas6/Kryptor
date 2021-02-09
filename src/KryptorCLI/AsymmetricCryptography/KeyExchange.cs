using Sodium;

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
    public static class KeyExchange
    {
        public static byte[] GetSharedSecret(byte[] privateKey, byte[] publicKey)
        {
            return ScalarMult.Mult(privateKey, publicKey);
        }

        public static byte[] GetEphemeralSharedSecret(byte[] publicKey, out byte[] ephemeralPublicKey)
        {
            using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
            byte[] ephemeralSharedSecret = ScalarMult.Mult(ephemeralKeyPair.PrivateKey, publicKey);
            ephemeralPublicKey = ephemeralKeyPair.PublicKey;
            return ephemeralSharedSecret;
        }

        public static byte[] GetPrivateKeySharedSecret(byte[] privateKey, out byte[] ephemeralPublicKey)
        {
            using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
            byte[] ephemeralSharedSecret = ScalarMult.Mult(privateKey, ephemeralKeyPair.PublicKey);
            ephemeralPublicKey = ephemeralKeyPair.PublicKey;
            return ephemeralSharedSecret;
        }
    }
}
