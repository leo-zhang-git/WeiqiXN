@echo off
setlocal EnableDelayedExpansion

:: ============================================================
:: Step 1: Request Admin Rights
:: ============================================================
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo.
    echo ============================================================
    echo   Requesting Administrator privileges...
    echo ============================================================
    echo.
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

:: ============================================================
:: Step 2: Initialize Variables
:: ============================================================
set "pythonCmd="
set "pythonPath="
set "candidate="

:: ============================================================
:: Step 3: Find Python Interpreter
:: ============================================================
for /f "delims=" %%i in ('where python 2^>nul') do (
    if not defined pythonCmd (
        set "candidate=%%~fi"
        if /i "!candidate:WindowsApps=!"=="!candidate!" (
            set "pythonCmd=python"
            set "pythonPath=!candidate!"
        )
    )
)

if not defined pythonCmd (
    for /f "delims=" %%i in ('where python3 2^>nul') do (
        if not defined pythonCmd (
            set "candidate=%%~fi"
            if /i "!candidate:WindowsApps=!"=="!candidate!" (
                set "pythonCmd=python3"
                set "pythonPath=!candidate!"
            )
        )
    )
)

if not defined pythonCmd (
    for /f "delims=" %%i in ('py -3 -c "import sys; print(sys.executable)" 2^>nul') do (
        if not defined pythonCmd (
            set "pythonCmd=py -3"
            set "pythonPath=%%i"
        )
    )
)

if not defined pythonCmd (
    echo Python not found
    pause
    exit /b 1
)

echo found python: %pythonPath%

:: ============================================================
:: Step 4: Install Dependencies
:: ============================================================
%pythonCmd% -m pip install --upgrade pip
%pythonCmd% -m pip install openpyxl

:: ============================================================
:: Step 5: Create DataJson Symbolic Link
:: ============================================================
echo.
echo Creating DataJson symbolic link...

:: Set paths based on script directory
set "scriptDir=%~dp0"
set "scriptDir=%scriptDir:~0,-1%"
set "linkPath=%scriptDir%\DataJson"
set "targetPath=%scriptDir%\..\UnityProject\Assets\Config\DataJson"

:: Create target directory if not exists
if not exist "%targetPath%" (
    echo Creating target directory: %targetPath%
    mkdir "%targetPath%" 2>nul
)

:: Create symbolic link
if exist "%linkPath%" (
    echo DataJson already exists at %linkPath%
) else (
    echo Creating symlink: %linkPath% -^> %targetPath%
    mklink /D "%linkPath%" "%targetPath%"
)

echo.
echo Setup complete
pause
