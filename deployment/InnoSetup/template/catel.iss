#define AppName "Catel"
#define SourceDirectory ""
; #define AppVersion "3.1"
#define AppVersion "[VERSION]"
; #define AppVersionAsText "3.1 beta 1"
#define AppVersionAsText "[VERSION_DISPLAY]"
#define Website "http://catel.codeplex.com"
#define OutputPrefix "catel"
#define Company "CatenaLogic"

#define VS9Root "{pf32}\Microsoft Visual Studio 9.0"
#define VS10Root "{pf32}\Microsoft Visual Studio 10.0"
#define VS11Root "{pf32}\Microsoft Visual Studio 11.0"

#define OutputFileWithSpaces OutputPrefix + "_" + AppVersionAsText
#define OutputFile StringChange(OutputFileWithSpaces, " ", "_")

[_ISTool]
EnableISX=false
Use7zip=false

[Setup]
AppName={#AppName}
AppVerName={#AppName} {#AppVersionAsText}
AppID={#AppName}
AppPublisher={#Company}
AppCopyright={#Company}
DefaultDirName={pf32}\{#AppName} {#AppVersion}
DefaultGroupName={#AppName}
UsePreviousSetupType=true
OutputDir=..\output
OutputBaseFilename={#OutputFile}
UninstallDisplayName={#AppName}
Compression=lzma2/Ultra64
UseSetupLdr=true
SolidCompression=true
ShowLanguageDialog=yes
VersionInfoVersion={#AppVersion}
AppVersion={#AppVersionAsText}
InternalCompressLevel=Ultra64
AppPublisherURL={#Website}
AppSupportURL={#Website}
AppUpdatesURL={#Website}
AppContact={#Website}
VersionInfoCompany={#Company}
AppMutex={#AppName}
LanguageDetectionMethod=none
DisableStartupPrompt=True
WizardImageFile=resources\[WIZARDIMAGEFILE].bmp
;WizardImageFile=resources\logo_large.bmp
;WizardImageFile=resources\logo_large_beta.bmp
;WizardImageFile=resources\logo_large_rc1.bmp
;WizardImageFile=resources\logo_large_rc2.bmp
WizardSmallImageFile=resources\logo_small.bmp
SetupIconFile=resources\catel.ico
UninstallDisplayIcon={app}\resources\catel.ico
SetupLogging=true
; For signing, the following sign tool must be configured
; Name: Signtool
; Command: "C:\Source\CatenaLogic_Certificates\Tools\signtool.exe" sign /t "http://timestamp.comodoca.com/authenticode" /f "C:\Source\CatenaLogic_Certificates\CodeSigning\current.pfx" "$f"
SignTool=Signtool

[InnoIDE_Settings]
UseRelativePaths=true

[Dirs]
Name: {app}\doc; 
Name: {app}\libraries; 
Name: {app}\resources;
Name: {app}\snippets; 
Name: {app}\templates; 

[Files]
Source: readme.txt; DestDir: {app};
Source: doc\catel.chm; DestDir: {app}\doc;
Source: resources\*; DestDir: {app}\resources;
Source: resources\catel.ico; DestDir: {app}\resources;

;----------
; Libraries
;----------

; Copy all files, then register specific files
Source: libraries\*; DestDir: {app}\libraries\; Flags: createallsubdirs recursesubdirs; Components: libraries; 

;----------
; Snippets
;----------

Source: snippets\C#\*; DestDir: {#VS9Root}\VC#\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9Installed;
Source: snippets\VB.NET\*; DestDir: {#VS9Root}\VB\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9Installed;
Source: snippets\C#\*; DestDir: {#VS9Root}\VC#\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9ExpressInstalled;
Source: snippets\VB.NET\*; DestDir: {#VS9Root}\VB\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9ExpressInstalled;
 
Source: snippets\C#\*; DestDir: {#VS10Root}\VC#\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10Installed;
Source: snippets\VB.NET\*; DestDir: {#VS10Root}\VB\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10Installed;
Source: snippets\C#\*; DestDir: {#VS10Root}\VC#\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10ExpressInstalled;
Source: snippets\VB.NET\*; DestDir: {#VS10Root}\VB\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10ExpressInstalled;
 
Source: snippets\C#\*; DestDir: {#VS11Root}\VC#\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11Installed;
Source: snippets\VB.NET\*; DestDir: {#VS11Root}\VB\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11Installed;
Source: snippets\C#\*; DestDir: {#VS11Root}\VC#\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11ExpressInstalled;
Source: snippets\VB.NET\*; DestDir: {#VS11Root}\VB\Snippets\1033\other\Catel; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11ExpressInstalled; 

;----------
; Templates
;----------

; VS 9 (VS 2008)
Source: templates\C#\itemtemplates\*; DestDir: {#VS9Root}\Common7\IDE\ItemTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9Installed; 
Source: templates\C#\projecttemplates\*; DestDir: {#VS9Root}\Common7\IDE\ProjectTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9Installed; 
Source: templates\VB.NET\itemtemplates\*; DestDir: {#VS9Root}\Common7\IDE\ItemTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9Installed; 
Source: templates\VB.NET\projecttemplates\*; DestDir: {#VS9Root}\Common7\IDE\ProjectTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9Installed;

; VS 9 (VS 2008 Express)
Source: templates\C#\itemtemplates\*; DestDir: {#VS9Root}\Common7\IDE\VCSExpress\ItemTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9ExpressInstalled; 
Source: templates\C#\projecttemplates\*; DestDir: {#VS9Root}\Common7\IDE\VCSExpress\ProjectTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9ExpressInstalled;
Source: templates\VB.NET\itemtemplates\*; DestDir: {#VS9Root}\Common7\IDE\VCSExpress\ItemTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9ExpressInstalled; 
Source: templates\VB.NET\projecttemplates\*; DestDir: {#VS9Root}\Common7\IDE\VCSExpress\ProjectTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs9ExpressInstalled;

; VS 10 (VS 2010)
Source: templates\C#\itemtemplates\*; DestDir: {#VS10Root}\Common7\IDE\ItemTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10Installed; 
Source: templates\C#\projecttemplates\*; DestDir: {#VS10Root}\Common7\IDE\ProjectTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10Installed;
Source: templates\VB.NET\itemtemplates\*; DestDir: {#VS10Root}\Common7\IDE\ItemTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10Installed; 
Source: templates\VB.NET\projecttemplates\*; DestDir: {#VS10Root}\Common7\IDE\ProjectTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10Installed;

; VS 10 (VS 2010 Express)
Source: templates\C#\itemtemplates\*; DestDir: {#VS10Root}\Common7\IDE\VCSExpress\ItemTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10ExpressInstalled; 
Source: templates\C#\projecttemplates\*; DestDir: {#VS10Root}\Common7\IDE\VCSExpress\ProjectTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10ExpressInstalled;
Source: templates\VB.NET\itemtemplates\*; DestDir: {#VS10Root}\Common7\IDE\VCSExpress\ItemTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10ExpressInstalled; 
Source: templates\VB.NET\projecttemplates\*; DestDir: {#VS10Root}\Common7\IDE\VCSExpress\ProjectTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs10ExpressInstalled;

; VS 11 (VS 11)
Source: templates\C#\itemtemplates\*; DestDir: {#VS11Root}\Common7\IDE\ItemTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11Installed;
Source: templates\C#\projecttemplates\*; DestDir: {#VS11Root}\Common7\IDE\ProjectTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11Installed;
Source: templates\VB.NET\itemtemplates\*; DestDir: {#VS11Root}\Common7\IDE\ItemTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11Installed;
Source: templates\VB.NET\projecttemplates\*; DestDir: {#VS11Root}\Common7\IDE\ProjectTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11Installed;

; VS 11 (VS 11 Express)
Source: templates\C#\itemtemplates\*; DestDir: {#VS11Root}\Common7\IDE\VCSExpress\ItemTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11ExpressInstalled;
Source: templates\C#\projecttemplates\*; DestDir: {#VS11Root}\Common7\IDE\VCSExpress\ProjectTemplates\CSharp\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11ExpressInstalled;
Source: templates\VB.NET\itemtemplates\*; DestDir: {#VS11Root}\Common7\IDE\VCSExpress\ItemTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11ExpressInstalled;
Source: templates\VB.NET\projecttemplates\*; DestDir: {#VS11Root}\Common7\IDE\VCSExpress\ProjectTemplates\VisualBasic\Catel\1033; Flags: createallsubdirs recursesubdirs; Components: templates; Check: IsVs11ExpressInstalled;

[CustomMessages]
DotNetMissing=This setup requires the .NET Framework. Please download and install the .NET Framework and run this setup again. Do you want to download the framework now?

[ThirdPartySettings]
CompileLogMethod=append

[UninstallDelete]
Name: {app}; Type: filesandordirs

[Registry]
Root: HKLM; SubKey: SOFTWARE\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Catel; ValueType: string; ValueData: {app}\Libraries\NET40; Flags: UninsDeleteKey
Root: HKLM; SubKey: SOFTWARE\Microsoft\.NETFramework\v4.5.40805\AssemblyFoldersEx\Catel; ValueType: string; ValueData: {app}\Libraries\NET45; Flags: UninsDeleteKey

Root: HKLM; SubKey: SOFTWARE\Microsoft\Microsoft SDKs\Silverlight\v4.0\AssemblyFoldersEx\Catel; ValueType: string; ValueData: {app}\Libraries\SL4; Flags: UninsDeleteKey
Root: HKLM; SubKey: SOFTWARE\Microsoft\Microsoft SDKs\Silverlight\v5.0\AssemblyFoldersEx\Catel; ValueType: string; ValueData: {app}\Libraries\SL5; Flags: UninsDeleteKey

Root: HKLM; SubKey: SOFTWARE\Microsoft\Microsoft SDKs\Silverlight for WindowsPhone\v4.0\AssemblyFoldersEx\Catel (WP7); ValueType: string; ValueData: {app}\Libraries\WP7; Flags: UninsDeleteKey
Root: HKLM; SubKey: SOFTWARE\Microsoft\Microsoft SDKs\WindowsPhone\v8.0\AssemblyFoldersEx\Catel (WP8); ValueType: string; ValueData: {app}\Libraries\WP8; Flags: UninsDeleteKey

;Root: HKLM; SubKey: SOFTWARE\Microsoft\Microsoft SDKs\Silverlight for WindowsPhone\v4.0\AssemblyFoldersEx\Catel (WinRT); ValueType: string; ValueData: {app}\Libraries\WinRT; Flags: UninsDeleteKey

[Icons]
Name: "{group}\Go to Catel homepage"; Filename: http://catel.codeplex.com;
Name: "{group}\Documentation (online)"; Filename: http://catel.catenalogic.com;
Name: "{group}\Documentation (offline)"; Filename: {app}\doc\catel.chm; 
Name: "{group}\Uninstall Catel"; Filename: {app}\unins000.exe; WorkingDir: {app}; IconFilename: {app}\resources\catel.ico; 

[Types]
Name: Full; Description: "Full installation";
Name: Custom; Description: Custom; Flags: IsCustom; 

[Components]
Name: libraries; Description: Libraries; Types: Full Custom;
Name: snippets; Description: "Code snippets"; Types: Full Custom; 
Name: templates; Description: "Project and Item templates"; Types: Full Custom;  

[Languages]
Name: "English"; MessagesFile: "compiler:Default.isl"
Name: "Czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "Danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "Dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "Finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "French"; MessagesFile: "compiler:Languages\French.isl"
Name: "German"; MessagesFile: "compiler:Languages\German.isl"
Name: "Hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "Italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "Japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "Norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "Polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "Portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "Russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "Spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Run]
; VS 9 (VS 2008)
Filename: {#VS9Root}\Common7\IDE\devenv.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2008 templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;
Filename: {#VS9Root}\Common7\IDE\VCSExpress.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2008 Express templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;
 
; VS 10 (VS 2010)
Filename: {#VS10Root}\Common7\IDE\devenv.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2010 templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;
Filename: {#VS10Root}\Common7\IDE\VCSExpress.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2010 Express templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;

; VS 11 (VS 2012) 
Filename: {#VS11Root}\Common7\IDE\devenv.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2012 templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;
Filename: {#VS11Root}\Common7\IDE\VCSExpress.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2012 Express templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist; 

[UninstallRun]
; VS 9 (VS 2008)
Filename: {#VS9Root}\Common7\IDE\devenv.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2008 templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;
Filename: {#VS9Root}\Common7\IDE\VCSExpress.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2008 Express templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;

; VS 10 (VS 2010) 
Filename: {#VS10Root}\Common7\IDE\devenv.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2010 templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;
Filename: {#VS10Root}\Common7\IDE\VCSExpress.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2010 Express templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;

; VS 11 (VS 2012) 
Filename: {#VS11Root}\Common7\IDE\devenv.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2012 templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;
Filename: {#VS11Root}\Common7\IDE\VCSExpress.exe; Parameters: /installvstemplates; StatusMsg: "Updating Visual Studio 2012 Express templates cache (can take a minute or two)"; Components: templates; Flags: SkipIfDoesntExist;

[Code]
//=========================================================================
// IsDotNetFrameworkInstalled
//=========================================================================
{
	Checks whether the right version of the .NET framework is installed
}

function IsDotNetFrameworkInstalled : Boolean;
var
    ErrorCode: Integer;
    NetFrameWorkInstalled : Boolean;
begin
	// Check if the .NET framework is installed
	//NetFrameWorkInstalled := RegKeyExists(HKLM,'SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727'); 	// 2.0
	//NetFrameWorkInstalled := RegKeyExists(HKLM,'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0'); 		// 3.0
	NetFrameWorkInstalled := RegKeyExists(HKLM,'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5'); 			// 3.5
	//NetFrameWorkInstalled := RegKeyExists(HKLM,'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4'); 			// 4.0

	// If the .NET framework is not installed, show message to user to download the framework
	if NetFrameWorkInstalled = false then
	begin
		if MsgBox(ExpandConstant('{cm:DotNetMissing}'), mbConfirmation, MB_YESNO) = idYes then
		begin
			ShellExec('open',
				//'http://www.microsoft.com/downloads/details.aspx?FamilyID=0856eacb-4362-4b0d-8edd-aab15c5e04f5&displaylang=en', // 2.0
				//'http://www.microsoft.com/downloads/details.aspx?FamilyID=10cc340b-f857-4a14-83f5-25634c3bf043&displaylang=en', // 3.0
				'http://www.microsoft.com/downloads/details.aspx?FamilyId=333325fd-ae52-4e35-b531-508d977d32a6&displaylang=en', // 3.5
				//'http://www.microsoft.com/downloads/details.aspx?familyid=9CFB2D51-5FF4-4491-B0E5-B386F32C0992&displaylang=en', // 4.0
				'','',SW_SHOWNORMAL,ewNoWait,ErrorCode);
		end;
	end;

	// Return result
	Result := NetFrameWorkInstalled;
end;

//=========================================================================
// INITIALIZESETUP
//=========================================================================
{
	This function initializes the setup.
}

function InitializeSetup(): Boolean;
var
  sPrevPath: String;
begin
  // Check .NET framework
  if (IsDotNetFrameworkInstalled() = false) then
  begin
	Result := false;
	exit;
  end;

  Result := true;
end;

//=========================================================================
// VS checks
//=========================================================================

function IsVs9Installed : Boolean;
var
  sPath: string;
begin
  sPath := ExpandConstant('{#VS9Root}\Common7\IDE\devenv.exe');
  
  Log('Checking path "' + sPath + '" to determine whether VS9 is installed');
    
  Result := FileExists(sPath);
end;

function IsVs9ExpressInstalled : Boolean;
var
  sPath: string;
begin
  sPath := ExpandConstant('{#VS9Root}\Common7\IDE\VCSExpress.exe');
  
  Log('Checking path "' + sPath + '" to determine whether VS9 Express is installed');
    
  Result := FileExists(sPath);
end;

//================================

function IsVs10Installed : Boolean;
var
  sPath: string;
begin
  sPath := ExpandConstant('{#VS10Root}\Common7\IDE\devenv.exe');
  
  Log('Checking path "' + sPath + '" to determine whether VS10 is installed');
  
  Result := FileExists(sPath);
end;

function IsVs10ExpressInstalled : Boolean;
var
  sPath: string;
begin
  sPath := ExpandConstant('{#VS10Root}\Common7\IDE\VCSExpress.exe');
  
  Log('Checking path "' + sPath + '" to determine whether VS10 Express is installed');
  
  Result := FileExists(sPath);
end;

//================================

function IsVs11Installed : Boolean;
var
  sPath: string;
begin
  sPath := ExpandConstant('{#VS11Root}\Common7\IDE\devenv.exe');
  
  Log('Checking path "' + sPath + '" to determine whether VS11 is installed');
  
  Result := FileExists(sPath);
end;

function IsVs11ExpressInstalled : Boolean;
var
  sPath: string;
begin
  sPath := ExpandConstant('{#VS11Root}\Common7\IDE\VCSExpress.exe');
  
  Log('Checking path "' + sPath + '" to determine whether VS11 Express is installed');
  
  Result := FileExists(sPath);
end;
