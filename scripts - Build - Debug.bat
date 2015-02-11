@echo off

IF NOT "%VS110COMNTOOLS%" == "" (call "%VS110COMNTOOLS%vsvars32.bat")
IF NOT "%VS120COMNTOOLS%" == "" (call "%VS120COMNTOOLS%vsvars32.bat")
IF NOT "%VS130COMNTOOLS%" == "" (call "%VS130COMNTOOLS%vsvars32.bat")
IF NOT "%VS140COMNTOOLS%" == "" (call "%VS140COMNTOOLS%vsvars32.bat")


for /F %%A in ('dir /b src\*.sln') do call devenv src\%%A /build "Debug"
pause