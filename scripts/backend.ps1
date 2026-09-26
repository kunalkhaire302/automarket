param([ValidateSet('run','build','test','migration-new','migration-recreate','migration-rewards','migrate','inspect','bootstrap-admin','seed-demo','inspect-demo-photos','reconcile-demo-migration')][string]$Action = 'run')
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$localConfig = Join-Path $root 'backend/.env.local'
if (Test-Path -LiteralPath $localConfig) {
    foreach ($line in Get-Content -LiteralPath $localConfig) {
        if ($line -match '^([A-Za-z_][A-Za-z0-9_]*)=(.*)$') {
            [Environment]::SetEnvironmentVariable($Matches[1], $Matches[2], 'Process')
        }
    }
}
$dotnet = Join-Path $root '.tools/dotnet/dotnet.exe'
if (!(Test-Path -LiteralPath $dotnet)) { $dotnet = 'dotnet' }
if (!$env:Jwt__Key) {
    $bytes = New-Object byte[] 48
    $rng = [Security.Cryptography.RandomNumberGenerator]::Create()
    $rng.GetBytes($bytes)
    $rng.Dispose()
    $env:Jwt__Key = [Convert]::ToBase64String($bytes)
}
Push-Location $root
try {
    switch ($Action) {
        'run' { & $dotnet run --project backend/src/Api --no-launch-profile }
        'build' { & $dotnet build backend/src/Api --no-restore }
        'test' { & $dotnet test backend/tests/Unit --no-restore }
        'migration-new' { & $dotnet tool run dotnet-ef migrations add InitialCreate --project backend/src/Infrastructure --startup-project backend/src/Api --output-dir Migrations }
        'migration-recreate' {
            & $dotnet tool run dotnet-ef migrations remove --project backend/src/Infrastructure --startup-project backend/src/Api --force
            if ($LASTEXITCODE -eq 0) { & $dotnet tool run dotnet-ef migrations add InitialCreate --project backend/src/Infrastructure --startup-project backend/src/Api --output-dir Migrations }
        }
        'migration-rewards' { & $dotnet tool run dotnet-ef migrations add RewardsLedger --project backend/src/Infrastructure --startup-project backend/src/Api --output-dir Migrations }
        'migrate' { & $dotnet tool run dotnet-ef database update --project backend/src/Infrastructure --startup-project backend/src/Api }
        'inspect' { & $dotnet run --project backend/tools/Database -- inspect }
        'bootstrap-admin' { & $dotnet run --project backend/tools/Database -- bootstrap-admin }
        'seed-demo' { & $dotnet run --project backend/tools/Database -- seed-demo }
        'inspect-demo-photos' { & $dotnet run --project backend/tools/Database -- inspect-demo-photos }
        'reconcile-demo-migration' { & $dotnet run --project backend/tools/Database -- reconcile-demo-migration }
    }
    if ($LASTEXITCODE -ne 0) { throw "Backend command failed with exit code $LASTEXITCODE." }
} finally { Pop-Location }
