@echo off
echo Installing System.Data.SQLite.Core package...
echo.

REM Check if NuGet is available
where nuget >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo NuGet CLI not found in PATH.
    echo.
    echo Please install the package using one of these methods:
    echo.
    echo METHOD 1 - Visual Studio Package Manager Console:
    echo   1. Open Visual Studio
    echo   2. Tools -^> NuGet Package Manager -^> Package Manager Console
    echo   3. Run: Install-Package System.Data.SQLite.Core -Version 1.0.118.0
    echo.
    echo METHOD 2 - Visual Studio NuGet UI:
    echo   1. Right-click TrySystem project in Solution Explorer
    echo   2. Select "Manage NuGet Packages..."
    echo   3. Browse for "System.Data.SQLite.Core"
    echo   4. Install version 1.0.118.0
    echo.
    pause
    exit /b 1
)

echo Restoring NuGet packages...
cd /d "%~dp0"
nuget restore TrySystem.sln

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Package restored successfully!
    echo You can now build the solution.
) else (
    echo.
    echo Package restore failed.
    echo Please use Visual Studio to install the package manually.
)

pause

