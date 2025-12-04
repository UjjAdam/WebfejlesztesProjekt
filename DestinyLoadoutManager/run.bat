@echo off
cd /d "%~dp0"

echo ========================================
echo  Destiny 2 Loadout Manager - Starting
echo ========================================
echo.

:: Stop any existing process
echo Stopping any running instances...
taskkill /IM dotnet.exe /F 2>nul
taskkill /IM DestinyLoadoutManager.exe /F 2>nul
timeout /t 2 /nobreak >nul

:: Clean old database for fresh start
echo Cleaning old database...
del /f /q "DestinyLoadoutManager.db" 2>nul
del /f /q "DestinyLoadoutManager.db-shm" 2>nul
del /f /q "DestinyLoadoutManager.db-wal" 2>nul

echo.
echo Building project...
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo  Application starting...
echo  Please wait for "Now listening on:" message
echo  Then open browser to:
echo    http://localhost:5000
echo    OR https://localhost:5001
echo ========================================
echo.

:: Run directly without PowerShell wrapper
dotnet run --no-build
