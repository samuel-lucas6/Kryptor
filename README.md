[![CodeFactor](https://www.codefactor.io/repository/github/kryptor-software/kryptor/badge)](https://www.codefactor.io/repository/github/kryptor-software/kryptor)

# Kryptor

Kryptor is free open source file encryption software for Windows. Linux and macOS support is being looked into. Kryptor is licensed under GPLv3.

Kryptor is designed to allow you to encrypt files before backing them up to an external drive or via a cloud storage provider such as Google Drive, OneDrive, etc. Many of the most popular cloud services are a privacy/security risk as the companies have access to your files. Kryptor can be used to prevent third parties (such as Google and Microsoft) from being able to access your uploaded files using encryption.

## Main Features:

- 100% offline besides automatically checking for updates (which can be disabled). No account needed.
- File encryption using XChaCha20 (default), XSalsa20, or AES-CBC with a 256-bit key.
- Key derivation using Password Hashing Competition winner Argon2.
- A unique encryption key per file derived from a password and/or keyfile.
- Optional anonymous renaming of encrypted files and folders.
- Built in password and passphrase generator. Auto clear clipboard after generation.
- Password strength checking using the zxcvbn library.
- Secure keyfile generation. Alternatively, any file type can be used as a keyfile.
- Password sharing support using libsodium Sealed Boxes (Curve25519, XSalsa20-Poly1305).
- Customisation of settings such as encryption algorithm, Argon2 parameters, light/dark theme, etc.
