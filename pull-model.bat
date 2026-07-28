@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

set MODEL=%1
if "%MODEL%"=="" set MODEL=llama3

echo.
echo  Ollama Model Downloader
echo.
echo  Model: %MODEL%
echo.

where ollama >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo  [!] Ollama not found. Install from https://ollama.ai
    pause
    exit /b 1
)

echo  Pull model '%MODEL%'? (Y/n):
set /p CONFIRM=
if /i "!CONFIRM!"=="n" exit /b 0

echo.
ollama pull %MODEL%
if %ERRORLEVEL% equ 0 (
    echo.
    echo  ✓ Download complete
) else (
    echo.
    echo  [!] Download failed
)
pause
