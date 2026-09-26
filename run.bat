@echo off
setlocal EnableExtensions

set "PROJECT_ROOT=%~dp0"
cd /d "%PROJECT_ROOT%"

echo.
echo ========================================
echo   AutoMarket development launcher
echo ========================================
echo.

if not exist "backend\.env.local" (
  echo ERROR: backend\.env.local is missing.
  echo Copy backend\.env.example to backend\.env.local and configure the database connection.
  pause
  exit /b 1
)

where powershell.exe >nul 2>nul
if errorlevel 1 (
  echo ERROR: Windows PowerShell was not found.
  pause
  exit /b 1
)

where node.exe >nul 2>nul
if errorlevel 1 (
  echo ERROR: Node.js was not found. Install Node.js 20 or newer.
  pause
  exit /b 1
)

where python.exe >nul 2>nul
if errorlevel 1 (
  echo ERROR: Python was not found. Install Python 3.11 or newer.
  pause
  exit /b 1
)

if not exist "frontend\node_modules" (
  echo Installing frontend dependencies...
  pushd "frontend"
  call npm install
  if errorlevel 1 (
    popd
    echo ERROR: Frontend dependency installation failed.
    pause
    exit /b 1
  )
  popd
)

python -c "import fastapi, uvicorn, pydantic" >nul 2>nul
if errorlevel 1 (
  echo Installing AI service dependencies...
  python -m pip install -r "ai-service\requirements.txt"
  if errorlevel 1 (
    echo ERROR: AI service dependency installation failed.
    pause
    exit /b 1
  )
)

if not exist "ai-service\.env.local" (
  echo NOTE: ai-service\.env.local is missing. AI model routes will use their safe not-configured fallback.
)

echo Starting backend on http://localhost:5100 ...
start "AutoMarket Backend" /D "%PROJECT_ROOT%" powershell.exe -NoExit -ExecutionPolicy Bypass -File "%PROJECT_ROOT%scripts\backend.ps1" run

echo Starting AI service on http://localhost:8000 ...
start "AutoMarket AI" /D "%PROJECT_ROOT%" powershell.exe -NoExit -ExecutionPolicy Bypass -File "%PROJECT_ROOT%scripts\ai.ps1"

echo Starting frontend on http://localhost:3000 ...
start "AutoMarket Frontend" /D "%PROJECT_ROOT%frontend" cmd.exe /k npm run dev

echo.
echo Services are starting in separate windows.
echo Close those windows or press Ctrl+C in each one to stop the project.
echo The browser will open shortly.
timeout /t 5 /nobreak >nul
start "" "http://localhost:3000"

endlocal
