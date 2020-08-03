# Kryptor

Kryptor is free open source file encryption software for Windows. Linux and Mac OS support is being looked into. Kryptor is licensed under GPLv3.

Kryptor is primarily designed to allow you to encrypt files before uploading them to a cloud service such as Google Drive, Microsoft OneDrive, Mega, Keybase, Nextcloud, etc. Many of the most popular cloud services are a privacy/security risk as they have direct access to your files and personal data, but Kryptor can be used to prevent third parties (such as Google and Microsoft) from being able to access your uploaded files using encryption.

## Main Features:

- 100% offline besides checking for updates (which can be disabled). No account needed.
- File encryption using XSalsa20, ChaCha20, AES-CBC, or AES-CTR with a 256-bit key.
- Key derivation using Password Hashing Competition winner Argon2.
- A unique encryption key per file derived from a password and/or keyfile.
- Encrypt then HMAC for file authentication.
- Password strength checking using the zxcvbn library.
- Built in password and passphrase generator. Auto clear clipboard after generation.
- Secure keyfile generation.
- Optional anonymous renaming of encrypted files and folders.
- Password sharing support using libsodium Sealed Boxes (Curve25519, XSalsa20-Poly1305).
- Customisation of settings such as encryption algorithm, Argon2 parameters, light/dark theme, etc.

      
