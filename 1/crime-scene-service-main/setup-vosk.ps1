# Download Vosk Russian model (vosk-model-ru-0.42) for speech recognition
# Run: cd e:\diploma_maga\1\crime-scene-service-main ; .\setup-vosk.ps1

$ErrorActionPreference = "Stop"
$Root = $PSScriptRoot
$ModelDir = Join-Path $Root "vosk-model-ru-0.42"
$Archive = Join-Path $Root "vosk-model-ru-0.42.zip"
$ArchivePart = "$Archive.part"
$Url = "https://alphacephei.com/vosk/models/vosk-model-ru-0.42.zip"
# Full zip is about 1.7 GB
$MinZipBytes = 1.5GB

if (Test-Path $ModelDir) {
    Write-Host "Vosk model already installed: $ModelDir"
    exit 0
}

$haveZip = (Test-Path $Archive) -and ((Get-Item $Archive).Length -ge $MinZipBytes)

if (-not $haveZip) {
    if (Test-Path $Archive) {
        Write-Host "Removing incomplete zip..."
        Remove-Item $Archive -Force
    }
    if (Test-Path $ArchivePart) {
        Remove-Item $ArchivePart -Force
    }

    Write-Host "Downloading Vosk model (~1.5 GB). This may take a while..."
    try {
        Invoke-WebRequest -Uri $Url -OutFile $ArchivePart -UseBasicParsing
        Move-Item -Path $ArchivePart -Destination $Archive -Force
    }
    catch {
        if (Test-Path $ArchivePart) { Remove-Item $ArchivePart -Force -ErrorAction SilentlyContinue }
        throw
    }
}
else {
    Write-Host "Zip already downloaded, skipping download."
}

Write-Host "Extracting archive (may take 5-15 min)..."
Expand-Archive -Path $Archive -DestinationPath $Root -Force
Remove-Item $Archive

Write-Host "Done. Model path: $ModelDir"
