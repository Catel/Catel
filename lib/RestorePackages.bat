for /f %%a IN ('dir /b ..\src\*.sln') do call ..\tools\nuget\nuget.exe restore ..\src\%%a -PackagesDirectory .\


pause