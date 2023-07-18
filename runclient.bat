@echo off
set PDIR=%~dp0
cd %PDIR%Bin\Content.Client
Content.Client.exe %*
cd %PDIR%
set PDIR=
pause