[![License: GPLv3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](http://www.gnu.org/licenses/gpl-3.0)
[![CodeQL](https://github.com/samuel-lucas6/Kryptor/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/samuel-lucas6/Kryptor/actions)
[![Specification](https://img.shields.io/badge/%23-specification-blueviolet)](https://www.kryptor.co.uk/technical-details)

# Kryptor
Kryptor is free and open source file encryption and signing software for Windows, Linux, and macOS.

It is a portable, cross-platform command line tool that makes use of modern and secure cryptographic algorithms. It aims to be a better version of [age](https://github.com/FiloSottile/age) and [Minisign](https://jedisct1.github.io/minisign/) to provide a simple, user friendly alternative to [GPG](https://gnupg.org/).

![kryptor](https://user-images.githubusercontent.com/63159663/148044395-868f26f5-5fc1-42f3-b357-b730ebb91d24.gif)

## Features
- Encrypt files/folders with a password, keyfile, or asymmetric keys.
- Encrypting files using a recipient's public key allows for authenticated, one-way file sharing.
- Create and verify digital signatures, with support for an authenticated comment and prehashing.
- Generation of asymmetric key pairs. The private key is encrypted for protection at rest.
- UNIX style password entry and random passphrase generation.
- Random keyfile generation. Any type of file can also be used as a keyfile.
- Encrypt file/folder names.
- Overwrite input files when encrypting files/folders.

For more information, please go to [kryptor.co.uk](https://www.kryptor.co.uk/).

## Usage
If you are just getting started, then I recommend reading the [tutorial](https://www.kryptor.co.uk/tutorial).
```
Usage: kryptor [options] [file]

Arguments:
  file                   specify a file/folder path

Options:
  -e|--encrypt           encrypt files/folders
  -d|--decrypt           decrypt files/folders
  -p|--password          specify a password (empty for interactive entry)
  -k|--keyfile           specify or randomly generate a keyfile
  -x|--private           specify your private key (unused or empty for default key)
  -y|--public            specify a public key
  -n|--names             encrypt file/folder names
  -o|--overwrite         overwrite files
  -g|--generate          generate a new key pair
  -r|--recover           recover your public key from your private key
  -s|--sign              create a signature
  -c|--comment           add a comment to a signature
  -l|--prehash           sign large files by prehashing
  -v|--verify            verify a signature
  -t|--signature         specify a signature file (unused for default name)
  -u|--update            check for updates
  -a|--about             view the program version and license
  -h|--help              show help information

Examples:
  --encrypt [file]
  --encrypt -p [file]
  --encrypt [-y recipient's public key] [file]
  --decrypt [-y sender's public key] [file]
  --sign [-c comment] [file]
  --verify [-y public key] [file]
  ```

### Specifying files
When referencing file names/paths that contain spaces, you must surround them with "speech marks":
```
$ kryptor -e -p "GitHub Logo.png"
$ kryptor -e -p "C:\Users\samuel-lucas6\Downloads\GitHub Logo.png"
```
Files in the same directory as the `kryptor` executable can be specified using their file name:
```
$ kryptor -e -p file.txt
```
However, files that are not in the same directory as the `kryptor` executable must be specified using a file path:
```
$ kryptor -e -p "C:\Users\samuel-lucas6\Documents\file.txt"
```
Multiple files and/or directories can be specified at once:
```
$ kryptor -e file1.txt file2.jpg file3.mp4 Photos Videos
```

### Specifying your private key
You can use the `-x|--private` option without specifying a file path to use your default private key. However, in most cases, this option does not need to be specified. For example, you can perform encryption, decryption, and signing with your default private key as follows:
```
$ kryptor -e file.txt
$ kryptor -d file.txt.kryptor
$ kryptor -s file.txt
```
This is the recommended approach, but it means your private keys must be kept in the default folder, which varies depending on your operating system:

- Windows: `%USERPROFILE%/.kryptor`
- Linux: `/home/.kryptor`
- macOS: `/Users/USERNAME/.kryptor`

To specify a private key for `-r|--recover` or a private key not stored in the default folder, you must use the `-x|--private` option followed by `:[file]` like so:
```
$ kryptor -r -x:"C:\Users\samuel-lucas6\Documents\encryption.private"
```
