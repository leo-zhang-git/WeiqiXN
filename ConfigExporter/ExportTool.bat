@echo off
chcp 65001 >nul
title 工程导表工具

:start
cls
echo ============================================================
echo                    工程导表工具
echo ============================================================
echo.
echo 功能说明:
echo   本工具用于将 Excel 表格导出为 Unity 配置文件
echo   - 自动校验 Excel 数据格式和类型
echo   - 导出 JSON 配置文件到 DataJson 目录
echo   - 导出 C# 数据类到 DataType 目录
echo   - 支持多 Sheet 页面导出
echo.
echo 表格文件要求:
echo   - 放置在 xlsx 文件夹中
echo   - Sheet 名称不能以 # 开头
echo   - 第1行: 列的中文名称(显示用)
echo   - 第2行: JSON key 名(变量名)
echo   - 第3行: 数据类型(string/int/float/boolean/list(...))
echo   - 第4行: 额外检查(以 # 开头)
echo   - 第5行起: 数据内容
echo.
echo ============================================================
echo.
echo 已有的表格文件:
dir /b xlsx\*.xlsx 2>nul
if "%errorlevel%"=="1" echo   (暂无)
echo.
echo ============================================================
echo.
set /p filename="请输入要导出的表格名(不含扩展名，输入 exit 退出): "

if /i "%filename%"=="exit" goto :eof
if /i "%filename%"=="" goto :start

python main.py %filename%

echo.
pause
goto :start
