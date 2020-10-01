#define MyAppName "Kryptor"
#define MyAppVersion "2.1.0 Beta"
#define VersionInfoVersion "2.1.0.0"
#define MyAppPublisher "Kryptor-Software"
#define Copyright "Copyright (C) 2020 Samuel Lucas"
#define MyAppURL "https://kryptor.co.uk"
#define MyAppExeName "Kryptor.exe"
#define KryptorDirectory "{userappdata}\Kryptor"
#define KryptorExtension ".kryptor"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{E6CA180D-E60D-4C84-A7A1-4DAF9A473A9D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
VersionInfoVersion = {#VersionInfoVersion}
AppCopyright = {#Copyright}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\Users\Developer\AppData\Roaming\{#MyAppName}
DisableDirPage=yes
DisableProgramGroupPage=yes
DisableReadyPage=yes
LicenseFile=C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\Inno LICENSE.txt
UninstallDisplayIcon={#KryptorDirectory}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}
OutputDir=C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Builds
OutputBaseFilename=Kryptor Setup
SetupIconFile=C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\Kryptor Icon.ico
ChangesAssociations=yes
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
// Flag file
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\first run.tmp"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion; Check: KryptorInstalled

// Kryptor executable
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\Kryptor (x86)\Kryptor.exe"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion; Check: "not IsWin64"
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\Kryptor (x64)\Kryptor.exe"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion; Check: IsWin64

// DLLs
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\libsodium (x86)\libsodium.dll"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion; Check: "not IsWin64" 
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\libsodium (x64)\libsodium.dll"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion; Check: IsWin64
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\Sodium.Core.dll"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\System.ValueTuple.dll"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\zxcvbn-core.dll"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\Ookii.Dialogs.WinForms.dll"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion

// Other files
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\wordlist.txt"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\settings.ini"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion

// License files
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\LICENSE.txt"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\libsodium LICENSE.txt"; DestDir: "{#KryptorDirectory}\Licenses"; Flags: ignoreversion
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\libsodium-net LICENSE.txt"; DestDir: "{#KryptorDirectory}\Licenses"; Flags: ignoreversion
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\zxcvbn LICENSE.txt"; DestDir: "{#KryptorDirectory}\Licenses"; Flags: ignoreversion
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\Ookii-Dialogs-Winforms LICENSE.txt"; DestDir: "{#KryptorDirectory}\Licenses"; Flags: ignoreversion

// Visual C++ Redistributable
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\vcruntime (x86)\vcruntime140.dll"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion; Check: "not IsWin64"
Source: "C:\Users\Developer\Desktop\Programming\Kryptor Inno Setup\Kryptor Files\vcruntime (x64)\vcruntime140.dll"; DestDir: "{#KryptorDirectory}"; Flags: ignoreversion; Check: IsWin64

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{#KryptorDirectory}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{#KryptorDirectory}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
// Associate extension with opening Kryptor
Root: HKCR; Subkey: "{#KryptorExtension}"; ValueData: "{#MyAppName}"; Flags: uninsdeletevalue; ValueType: string; ValueName: ""
Root: HKCR; Subkey: "{#MyAppName}"; ValueData: "Program {#MyAppName}";  Flags: uninsdeletekey; ValueType: string; ValueName: ""
Root: HKCR; Subkey: "{#MyAppName}\DefaultIcon"; ValueData: "{#KryptorDirectory}\{#MyAppExeName},0"; ValueType: string; ValueName: ""
Root: HKCR; Subkey: "{#MyAppName}\shell\open\command"; ValueData: """{#KryptorDirectory}\{#MyAppExeName}"" ""%1"""; ValueType: string; ValueName: ""

[UninstallDelete]
// Delete Kryptor folder on uninstall
Type: filesandordirs; Name: "{#KryptorDirectory}"

[Code]
// Don't extract 'first run.tmp' when updating Kryptor
function KryptorInstalled: Boolean;
   begin
     if (FileExists(ExpandConstant('{app}\{#MyAppExeName}'))) then
     begin
       Result := False;
     end
     else
     begin
      Result := True;
     end;
   end;


