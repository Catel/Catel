REM Deleting packages
for /d %%p in (".\lib\*.*") do rmdir "%%p" /s /q

REM Deleting output
rmdir .\output /s /q

pause