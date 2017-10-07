; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Media Librarian"
#define MyAppVersion "1.0"
#define MyAppPublisher "Alexander (LinJay) Lysenko"
#define MyAppURL "https://github.com/Alexander-Lysenko/MediaLibrarian"
#define MyAppExeName "MediaLibrarian.exe"
#define SourceFolder "C:\Users\LinJay\Desktop\MediaLibrarian\"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{165D8C3B-25E2-4266-AD31-A7DDEA0A5E7F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableWelcomePage=no
AllowNoIcons=yes
LicenseFile={#SourceFolder}MediaLibrarian\license.txt
UninstallDisplayIcon={#SourceFolder}MediaLibrarian\icon.ico
OutputDir={#SourceFolder}
OutputBaseFilename=MediaLibrarianSetup
SetupIconFile={#SourceFolder}MediaLibrarian\icon.ico
Compression=lzma
SolidCompression=yes


[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "{#SourceFolder}MediaLibrarian\bin\Release\MediaLibrarian.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceFolder}MediaLibrarian\bin\Release\EntityFramework.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceFolder}MediaLibrarian\bin\Release\EntityFramework.SqlServer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceFolder}MediaLibrarian\bin\Release\Help.chm"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceFolder}MediaLibrarian\bin\Release\System.Data.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceFolder}MediaLibrarian\bin\Release\System.Data.SQLite.EF6.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceFolder}MediaLibrarian\bin\Release\System.Data.SQLite.Linq.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceFolder}MediaLibrarian\bin\Release\x86\*"; DestDir: "{app}\x86"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#SourceFolder}MediaLibrarian\bin\Release\x64\*"; DestDir: "{app}\x64"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Name: "{app}\*.*"; Type: filesandordirs
Name: (app); Type: dirifempty