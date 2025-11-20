# NuGet Package Restore Script
Write-Host "Restoring NuGet packages..." -ForegroundColor Green

$solutionPath = Join-Path $PSScriptRoot "TrySystem.sln"
$projectPath = Join-Path $PSScriptRoot "TrySystem\TrySystem.csproj"

# Check if NuGet.exe exists
$nugetPath = Join-Path $PSScriptRoot ".nuget\NuGet.exe"
if (-not (Test-Path $nugetPath)) {
    Write-Host "Downloading NuGet.exe..." -ForegroundColor Yellow
    $nugetDir = Join-Path $PSScriptRoot ".nuget"
    if (-not (Test-Path $nugetDir)) {
        New-Item -ItemType Directory -Path $nugetDir | Out-Null
    }
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nugetPath
}

# Restore packages
Write-Host "Restoring packages for solution..." -ForegroundColor Yellow
& $nugetPath restore $solutionPath

if ($LASTEXITCODE -eq 0) {
    Write-Host "Packages restored successfully!" -ForegroundColor Green
} else {
    Write-Host "Package restore failed. Trying to install SQLite package directly..." -ForegroundColor Yellow
    & $nugetPath install System.Data.SQLite.Core -Version 1.0.118.0 -OutputDirectory packages -SolutionDirectory $PSScriptRoot
}

Write-Host "Done!" -ForegroundColor Green

