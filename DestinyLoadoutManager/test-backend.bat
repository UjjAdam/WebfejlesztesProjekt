@echo off
echo ========================================
echo  Backend Connection Test
echo ========================================
echo.

echo Testing if backend is running...
echo.

curl -s http://localhost:5000 >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [OK] Backend is RUNNING on http://localhost:5000
) else (
    echo [ERROR] Backend is NOT RUNNING!
    echo Please start the backend first using run.bat
)

echo.

curl -s http://localhost:5000/Home/DebugAjax
if %ERRORLEVEL% EQU 0 (
    echo [OK] Backend API is responding
) else (
    echo [ERROR] Backend API is not responding
)

echo.
echo ========================================
pause
