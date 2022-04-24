/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2022 Samuel Lucas

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

using Sodium;

namespace Kryptor;

public static class KeyExchange
{
    public static byte[] GetSharedSecret(byte[] privateKey, byte[] publicKey)
    {
        byte[] sharedSecret = ScalarMult.Mult(privateKey, publicKey);
        return GenericHash.Hash(Arrays.Concat(sharedSecret, ScalarMult.Base(privateKey), publicKey), key: null, Constants.EncryptionKeyLength);
    }

    public static byte[] GetPublicKeySharedSecret(byte[] publicKey, out byte[] ephemeralPublicKey)
    {
        using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
        ephemeralPublicKey = ephemeralKeyPair.PublicKey;
        byte[] ephemeralSharedSecret = ScalarMult.Mult(ephemeralKeyPair.PrivateKey, publicKey);
        return GenericHash.Hash(Arrays.Concat(ephemeralSharedSecret, ephemeralPublicKey, publicKey), key: null, Constants.EncryptionKeyLength);
    }

    public static byte[] GetPrivateKeySharedSecret(byte[] privateKey, out byte[] ephemeralPublicKey)
    {
        using var ephemeralKeyPair = PublicKeyBox.GenerateKeyPair();
        ephemeralPublicKey = ephemeralKeyPair.PublicKey;
        byte[] ephemeralSharedSecret = ScalarMult.Mult(privateKey, ephemeralKeyPair.PublicKey);
        return GenericHash.Hash(Arrays.Concat(ephemeralSharedSecret, ScalarMult.Base(privateKey), ephemeralPublicKey), key: null, Constants.EncryptionKeyLength);
    }
}