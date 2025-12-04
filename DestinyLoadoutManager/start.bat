@echo off
cd /d "%~dp0"
echo Starting Destiny 2 Loadout Manager...
echo.
dotnet build
echo.
echo Application starting on http://localhost:5000
echo Press Ctrl+C to stop the server
echo.
dotnet run
pause
