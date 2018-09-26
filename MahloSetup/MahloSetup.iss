[Setup]
AppName=MahloMapper
AppVersion=1.1
PrivilegesRequired=none
AppId={{743E0273-076F-4EB5-BCDE-4FD08FE1034D}
DefaultDirName={pf}\PA-Group\MahloMapper
AppPublisher=PA-Group, USA
AppContact=John Kendall
AppSupportPhone=423-473-7541
OutputBaseFilename=MahloMapperSetup
UninstallDisplayName=Mahlo Mapper
VersionInfoVersion=1.1
VersionInfoCompany=PA-Group, USA
VersionInfoDescription=Broadloom Coater Monitor

[Types]
Name: "Mahlo2"; Description: "Mahlo 2"
Name: "BowAndSkew"; Description: "Bow and Skew"
Name: "PatternRepeat"; Description: "Pattern Repeat"
Name: "client"; Description: "All Three Clients"
Name: "server"; Description: "Server Installation"

[Components]
Name: "Mahlo2"; Description: "Mahlo 2 Unique Components"; Types: Mahlo2 client server
Name: "BowAndSkew"; Description: "Bow and Skew Unique Components"; Types: BowAndSkew client server
Name: "PatternRepeat"; Description: "Pattern Repeat Unique Components"; Types: PatternRepeat client server
Name: "MahloClient"; Description: "Mahlo Client Program"; Types: Mahlo2 BowAndSkew PatternRepeat client server
Name: "MahloService"; Description: "Mahlo Service Program"; Types: server

[Files]
;MahloClient files
Source: "..\MahloClient\bin\Release\MahloClient.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloClient MahloService
Source: "..\MahloClient\bin\Release\MahloClient.exe.config"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloClient MahloService
Source: "..\MahloClient\bin\Release\MahloClient.pdb"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloClient MahloService
Source: "..\MahloClient\App.ico"; DestDir: "{app}"; Components: MahloClient MahloService
Source: "..\MahloClient\Camera.ico"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloClient MahloService
Source: "..\MahloClient\Rulers.ico"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloClient MahloService
; MahloService files
Source: "..\MahloService\bin\Release\MahloService.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\MahloService.exe.config"; DestDir: "{app}"; Flags: ignoreversion onlyifdoesntexist; Components: MahloService
Source: "..\MahloService\bin\Release\MahloService.pdb"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
; MahloClient DLLs
Source: "..\MahloClient\bin\Release\Microsoft.AspNet.SignalR.Client.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloClient MahloService
Source: "..\MahloClient\bin\Release\System.ValueTuple.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloClient MahloService
; DLLs used by both MahloClient and MahloService
Source: "..\MahloService\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService MahloClient
Source: "..\MahloService\bin\Release\PropertyChanged.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService MahloClient
Source: "..\MahloService\bin\Release\SimpleInjector.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService MahloClient
Source: "..\MahloService\bin\Release\System.Reactive.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService MahloClient
; MahloService DLLs
Source: "..\MahloService\bin\Release\System.ValueTuple.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\System.Web.Cors.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\App_Web_OpcLabs.EasyOpcClassicRaw.amd64.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\App_Web_OpcLabs.EasyOpcClassicRaw.x86.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Dapper.Contrib.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Dapper.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Dapper.FluentColumnMapping.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Abstractions.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Extensions.SqlAnywhere.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Extensions.SqlServer.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.Core.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.Db2.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.Firebird.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.Hana.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.Jet.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.MySql.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.Oracle.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.Postgres.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.Redshift.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.SqlAnywhere.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.SQLite.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.SqlServer.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\FluentMigrator.Runner.SqlServerCe.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Microsoft.AspNet.SignalR.Core.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Microsoft.Owin.Cors.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Microsoft.Owin.Diagnostics.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Microsoft.Owin.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Microsoft.Owin.Host.HttpListener.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Microsoft.Owin.Hosting.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Microsoft.Owin.Security.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcComRcw.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcLabs.BaseLib.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcLabs.BaseLibExtensions.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcLabs.EasyOpc.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcLabs.EasyOpcClassic.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcLabs.EasyOpcClassicInternal.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcLabs.EasyOpcClassicNative.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcLabs.EasyOpcClassicNetApi.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcNetApi.Com.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcNetApi.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\OpcNetApi.Xml.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Owin.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Serilog.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Serilog.Settings.AppSettings.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Serilog.Sinks.Console.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Serilog.Sinks.EventLog.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Serilog.Sinks.File.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\System.Data.SqlServerCe.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService
Source: "..\MahloService\bin\Release\Westwind.Utilities.Configuration.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: MahloService

[Icons]
Name: "{commondesktop}\Mahlo2"; Filename: "{app}\MahloClient.exe"; IconFilename: "{app}\Rulers.ico"; Parameters: "Mahlo"; Components: Mahlo2
Name: "{commondesktop}\BowAndSkew"; Filename: "{app}\MahloClient.exe"; IconFilename: "{app}\Rulers.ico"; IconIndex: 0; Parameters: "BowAndSkew"; Components: BowAndSkew
Name: "{commondesktop}\PatternRepeat"; Filename: "{app}\MahloClient.exe"; IconFilename: "{app}\Camera.ico"; IconIndex: 0; Parameters: "PatternRepeat"; Components: PatternRepeat
Name: "{group}\Mahlo2"; Filename: "{app}\MahloClient.exe"; IconFilename: "{app}\Rulers.ico"; Parameters: "Mahlo"; Components: Mahlo2
Name: "{group}\BowAndSkew"; Filename: "{app}\MahloClient.exe"; IconFilename: "{app}\Rulers.ico"; IconIndex: 0; Parameters: "BowAndSkew"; Components: BowAndSkew
Name: "{group}\PatternRepeat"; Filename: "{app}\MahloClient.exe"; IconFilename: "{app}\Camera.ico"; IconIndex: 0; Parameters: "PatternRepeat"; Components: PatternRepeat

[ThirdParty]
UseRelativePaths=True

[Run]
Filename: "{app}\MahloService.exe"; Parameters: "--install"; Components: MahloService

[UninstallRun]
Filename: "{app}\MahloService.exe"; Parameters: "--uninstall"; Components: MahloService
