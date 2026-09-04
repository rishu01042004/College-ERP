@echo off
cd /d "%~dp0src\CollegeERP.Api"
dotnet restore
dotnet run
pause
