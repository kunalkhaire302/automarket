$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$serviceRoot = Join-Path $root 'ai-service'
$localConfig = Join-Path $serviceRoot '.env.local'

if (Test-Path -LiteralPath $localConfig) {
    foreach ($line in Get-Content -LiteralPath $localConfig) {
        if ($line -match '^([A-Za-z_][A-Za-z0-9_]*)=(.*)$') {
            [Environment]::SetEnvironmentVariable($Matches[1], $Matches[2], 'Process')
        }
    }
}

Push-Location $serviceRoot
try {
    python -m uvicorn app.main:app --reload --host 127.0.0.1 --port 8000
    if ($LASTEXITCODE -ne 0) { throw "AI service exited with code $LASTEXITCODE." }
} finally {
    Pop-Location
}
