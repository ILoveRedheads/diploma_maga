# Run DB migrations and start API on port 5197
# Run: cd e:\diploma_maga\1\crime-scene-service-main ; .\setup-and-run.ps1

$ErrorActionPreference = "Stop"
$Root = $PSScriptRoot

if (-not (Test-Path (Join-Path $Root "vosk-model-ru-0.42"))) {
    Write-Host "Vosk model not found. Running setup-vosk.ps1 ..."
    & (Join-Path $Root "setup-vosk.ps1")
}

Write-Host "Applying SQLite migrations..."
Push-Location (Join-Path $Root "src\CSService.Migrations")
dotnet run
if ($LASTEXITCODE -ne 0) { Pop-Location; exit $LASTEXITCODE }
Pop-Location

Write-Host "Starting API at http://0.0.0.0:5197 ..."
Push-Location (Join-Path $Root "src\CSService.API")
dotnet run --urls "http://0.0.0.0:5197"
