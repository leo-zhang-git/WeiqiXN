@echo off
chcp 65001 >nul
setlocal EnableDelayedExpansion

:: ============================================================
:: 步骤 1: 请求管理员权限
:: ============================================================
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo.
    echo ============================================================
    echo   正在请求管理员权限...
    echo ============================================================
    echo.
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

:: ============================================================
:: 步骤 2: 初始化变量
:: ============================================================
set "pythonCmd="
set "pythonPath="
set "candidate="

:: ============================================================
:: 步骤 3: 查找 Python 解释器
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
    echo 未找到 Python
    pause
    exit /b 1
)

echo 已找到 Python: %pythonPath%

:: ============================================================
:: 步骤 4: 安装依赖项
:: ============================================================
%pythonCmd% -m pip install --upgrade pip
%pythonCmd% -m pip install openpyxl

:: ============================================================
:: 步骤 5: 创建 DataJson 符号链接
:: ============================================================
echo.
echo 正在创建 DataJson 符号链接...

:: Set paths based on script directory
set "scriptDir=%~dp0"
set "scriptDir=%scriptDir:~0,-1%"
set "linkPath=%scriptDir%\DataJson"
set "targetPath=%scriptDir%\..\UnityProject\Assets\Config\DataJson"

:: Create target directory if not exists
if not exist "%targetPath%" (
    echo 正在创建目标目录: %targetPath%
    mkdir "%targetPath%" 2>nul
)

:: Create symbolic link
if exist "%linkPath%" (
    echo DataJson 已在 %linkPath% 存在
) else (
    echo 正在创建符号链接: %linkPath% -^> %targetPath%
    mklink /D "%linkPath%" "%targetPath%"
)

:: ============================================================
:: 步骤 6: 创建 DataType 符号链接
:: ============================================================
echo.
echo 正在创建 DataType 符号链接...

set "linkPath=%scriptDir%\DataType"
set "targetPath=%scriptDir%\..\UnityProject\Assets\Config\DataType"

if not exist "%targetPath%" (
    echo 正在创建目标目录: %targetPath%
    mkdir "%targetPath%" 2>nul
)

if exist "%linkPath%" (
    echo DataType 已在 %linkPath% 存在
) else (
    echo 正在创建符号链接: %linkPath% -^> %targetPath%
    mklink /D "%linkPath%" "%targetPath%"
)

echo.
echo 安装完成
pause