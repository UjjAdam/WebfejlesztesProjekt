@echo off
setlocal enabledelayedexpansion
cd /d "%~dp0"

color 0A
cls

echo ========================================
echo  Destiny 2 Loadout Manager - Starting
echo ========================================
echo.

:: Stop any existing process
echo Stopping any running instances...
taskkill /IM dotnet.exe /F 2>nul
taskkill /IM DestinyLoadoutManager.exe /F 2>nul
timeout /t 2 /nobreak >nul

echo.
echo Building project...
dotnet build
if !ERRORLEVEL! NEQ 0 (
    color 0C
    echo.
    echo [ERROR] Build failed! Please check the errors above.
    echo.
    pause
    exit /b 1
)

echo.
color 0A
echo ========================================
echo  Application building and starting...
echo  Please wait for "Now listening on:" message
echo ========================================
echo.

:: Restore and clean
echo Restoring NuGet packages...
dotnet restore
if !ERRORLEVEL! NEQ 0 (
    color 0C
    echo [ERROR] Package restore failed!
    pause
    exit /b 1
)

echo.
echo Cleaning previous build artifacts...
dotnet clean --no-build 2>nul

echo.
echo ========================================
echo  Running application...
echo.
echo Access the application at:
echo   - http://localhost:5000
echo   - https://localhost:5001
echo.
echo Press Ctrl+C to stop the server
echo ========================================
echo.

:: Run the application
dotnet run --no-build

color 07
echo.
echo Application stopped.
pause
endlocal
