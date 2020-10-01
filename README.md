[![CodeFactor](https://www.codefactor.io/repository/github/kryptor-software/kryptor/badge)](https://www.codefactor.io/repository/github/kryptor-software/kryptor)

# Kryptor

Kryptor is free and open source file encryption software for Windows, Linux, and macOS. Kryptor is licensed under [GPLv3](https://github.com/Kryptor-Software/Kryptor/blob/master/LICENSE).

![Screenshot](https://kryptor.co.uk/Screenshots/File%20Encryption.gif)

## Main Features

- File encryption using XChaCha20 (default), XSalsa20, or AES-CBC with 256-bit keys.
- Key derivation using the Password Hashing Competition winner Argon2.
- A unique encryption key per file derived from a password and/or keyfile.
- Secure keyfile generation. Alternatively, any file type can be used as a keyfile.
- Optional anonymous renaming of encrypted files and folders.
- Memory encryption to protect sensitive data in memory.
- Password sharing support using libsodium Sealed Boxes (Curve25519, XSalsa20-Poly1305).
- Built in password and passphrase generator. Auto clear clipboard after copying passwords.
- Customisation of settings such as encryption algorithm, Argon2 parameters, light/dark theme, etc.
- Can be run offline and no account is needed. By default, there are automatic checks for updates, but this can be disabled.

For more information, head over to the [Kryptor website](https://kryptor.co.uk).

## Building Kryptor

Before you do anything with the source code, make sure you understand the [GPLv3](https://www.gnu.org/licenses/gpl-3.0.en.html) license used by Kryptor. Click [here](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) for a summary of GPLv3.

1. Click the green 'Code' button and 'Download ZIP'.
2. Extract the ZIP, navigate to the 'Kryptor/src' folder, then open the 'Kryptor.sln' file in [Visual Studio 2019 Community](https://visualstudio.microsoft.com/vs/community/).
3. You may be presented with lots of errors, but don't worry. You can go to Build => [Clean Solution](https://docs.microsoft.com/en-us/visualstudio/ide/building-and-cleaning-projects-and-solutions-in-visual-studio?view=vs-2019), select [Release and x64](https://docs.microsoft.com/en-us/cpp/build/working-with-project-properties?view=vs-2019) for the build options, and then click Build => Build Kryptor. Next, run the program. This should resolve all of the errors.
4. The [libsodium-core](https://github.com/tabrath/libsodium-core/issues/44) library used by Kryptor does not support building to AnyCPU in Visual Studio - you must either build to x86 or x64. Build to x64 when possible.

#### Requirements
- Windows: If you encounter any unhandled exceptions due to 'Sodium.Core' or 'libsodium', then you will need to either install the [Visual C++ Redistributable for Visual Studio 2015-2019](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads) or copy the 'vcruntime140.dll' from 'Kryptor/src/Kryptor/bin/(x86 or x64 depending on your build)/Release/' to the folder containing the 'Kryptor.exe' file. This runtime is required for libsodium to work - this '.dll' file must stay with the executable on Windows.
- Linux & macOS: You will want to download the latest Kryptor [release](https://kryptor.co.uk/Downloads.html) for your platform. For Linux, you'll need to copy the 'libsodium.so' file to the same folder as the 'Kryptor.exe' file you've built. For macOS, you'll need to do the same but with the 'libsodium.dylib' file.

#### Notes
- [Visual Studio 2019 Community](https://visualstudio.microsoft.com/vs/community/) is the IDE I recommend building with, but be aware that it isn't open source, and you have to sign into a Microsoft account after 30 days ([solution](https://github.com/beatcracker/VSCELicense)). If you just want to view the code, then you can use [VSCodium](https://vscodium.com/), which is the open source version of VSCode. However, this won't allow you to view the Windows Forms Designer.

## Donate

You can support the developer of Kryptor by donating through Paypal, GitHub, Bitcoin, or Monero.

Donations will go towards buying a code signing certificate (£65/yr) to remove the 'Unknown Publisher' message on Windows. Any excess donations will help pay for website hosting (£50/yr) and the website domain (£8/yr). Find out how to donate [here](https://kryptor.co.uk/Donate.html).
