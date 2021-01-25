using Sodium;

/*
    Kryptor: Free and open source file encryption.
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
    public static class KeyExchange
    {
        public static byte[] GetLongTermSharedSecret(byte[] privateKey, byte[] publicKey)
        {
            privateKey = ConvertPrivateKeyToCurve25519(privateKey);
            publicKey = ConvertPublicKeyToCurve25519(publicKey);
            return GetSharedSecret(privateKey, publicKey);
        }

        public static byte[] GetSharedSecret(byte[] privateKey, byte[] publicKey)
        {
            return ScalarMult.Mult(privateKey, publicKey);
        }

        public static (byte[] ephemeralSharedSecret, byte[] ephemeralPublicKey) GetEphemeralSharedSecret(byte[] publicKey)
        {
            using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
            byte[] ephemeralSharedSecret = ScalarMult.Mult(ephemeralKeyPair.PrivateKey, publicKey);
            return (ephemeralSharedSecret, ephemeralKeyPair.PublicKey);
        }

        private static byte[] ConvertPublicKeyToCurve25519(byte[] publicKey)
        {
            return PublicKeyAuth.ConvertEd25519PublicKeyToCurve25519PublicKey(publicKey);
        }

        private static byte[] ConvertPrivateKeyToCurve25519(byte[] privateKey)
        {
            return PublicKeyAuth.ConvertEd25519SecretKeyToCurve25519SecretKey(privateKey);
        }
    }
}
