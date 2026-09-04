$ErrorActionPreference = "Stop"
Set-Location "$PSScriptRoot\src\CollegeERP.Api"
dotnet restore
dotnet run
