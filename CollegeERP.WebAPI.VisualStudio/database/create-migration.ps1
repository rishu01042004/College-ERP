$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot\..\src\CollegeERP.Api"
dotnet tool install --global dotnet-ef --version 8.* 2>$null
if ($LASTEXITCODE -ne 0) { dotnet tool update --global dotnet-ef --version 8.* }
dotnet ef migrations add InitialCreate --output-dir Data/Migrations
dotnet ef database update
