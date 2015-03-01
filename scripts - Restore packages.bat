for /f %%a in ('dir /b src\*.sln') do call tools\nuget\nuget.exe restore src\%%a -PackagesDirectory .\lib


pause