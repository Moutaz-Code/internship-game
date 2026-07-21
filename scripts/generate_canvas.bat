@echo off
setlocal

set "PYTHONDONTWRITEBYTECODE=1"
set "SCRIPT=%~dp0generate_obsidian_canvas.py"

if exist "%LOCALAPPDATA%\Programs\Python\Python314\python.exe" (
    "%LOCALAPPDATA%\Programs\Python\Python314\python.exe" "%SCRIPT%" %*
    exit /b %ERRORLEVEL%
)

for /f "delims=" %%P in ('dir /b /s "%LOCALAPPDATA%\Programs\Python\Python*\python.exe" 2^>nul') do (
    "%%P" "%SCRIPT%" %*
    exit /b %ERRORLEVEL%
)

where py >nul 2>nul
if %ERRORLEVEL% EQU 0 (
    py -3 "%SCRIPT%" %*
    exit /b %ERRORLEVEL%
)

where python >nul 2>nul
if %ERRORLEVEL% EQU 0 (
    python "%SCRIPT%" %*
    exit /b %ERRORLEVEL%
)

echo Could not find a usable Python interpreter.
echo Install Python or run scripts\generate_obsidian_canvas.py with a full Python path.
exit /b 1
