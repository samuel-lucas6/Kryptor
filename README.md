[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](http://www.gnu.org/licenses/gpl-3.0)
[![CodeQL](https://github.com/samuel-lucas6/Kryptor/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/samuel-lucas6/Kryptor/actions)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=samuel-lucas6_Kryptor&metric=security_rating)](https://sonarcloud.io/dashboard?id=samuel-lucas6_Kryptor)

# Kryptor

Kryptor is free and open source file encryption software for Windows, Linux, and macOS.

It is a portable, cross-platform command line tool that makes use of modern and secure cryptographic algorithms. It aims to be a mixture of [age](https://github.com/FiloSottile/age) and [Minisign](https://github.com/jedisct1/minisign) in order to provide a simple, user friendly alternative to [GPG](https://gnupg.org/).

![kryptor](https://user-images.githubusercontent.com/63159663/110021517-c0af3500-7d22-11eb-9acd-02ba9f24617c.gif)

## Features

- Encrypt files/folders with a password, keyfile, or asymmetric keys.
- Create and verify digital signatures. Supports an authenticated comment and prehashing for large files.
- Generation of asymmetric key pairs. The private key is encrypted for protection at rest.
- UNIX style password entry and random passphrase generation.
- Encrypting files using a recipient's public key allows for authenticated file sharing.
- Random keyfile generation. Any type of file can also be used as a keyfile.
- Optional obfuscation of output file names when encrypting files/folders.
- Optional overwriting of input files.

For more information, go to [kryptor.co.uk](https://www.kryptor.co.uk/).

## Usage
If you are just getting started, I recommend reading the [tutorial](https://www.kryptor.co.uk/tutorial).
```
Usage: kryptor [options] [file]

Arguments:
  file                   specify a file path

Options:
  -e|--encrypt           encrypt files/folders
  -d|--decrypt           decrypt files/folders
  -p|--password          use a password
  -k|--keyfile           specify a keyfile
  -x|--private           specify your private key (blank for default)
  -y|--public            specify a public key
  -f|--obfuscate         obfuscate file names
  -o|--overwrite         overwrite input files
  -g|--generate          generate a new key pair
  -r|--recover           recover your public key from your private key
  -s|--sign              create a signature
  -c|--comment           add a comment to a signature
  -l|--prehash           sign large files by prehashing
  -v|--verify            verify a signature
  -t|--signature         specify a signature file
  -u|--update            check for updates
  -a|--about             view the program version and license
  -h|--help              show help information

Examples:
  --encrypt -p [file]
  --encrypt -x [file]
  --encrypt -x [-y recipient public key] [file]
  --decrypt -x [-y sender recipient key] [file]
  --sign -x [-c comment] [file]
  --verify [-y public key] [-t signature] [file]
  ```
### Specifying files
When referencing file paths/file names that contain spaces, you must surround them with 'apostrophes' on Linux/macOS and "speech marks" on Windows:
```
$ kryptor -e -p 'GitHub Logo.png'
$ kryptor -e -p '/home/samuel/Downloads/GitHub Logo.png'
```
Files in the same directory as the ```kryptor``` executable can be specified using their file name:
```
$ kryptor -e -p message.txt
```
However, files that are not in the same directory as the ```kryptor``` executable must be specified using a file path:
```
$ kryptor -e -p /home/samuel/Documents/message.txt
```
