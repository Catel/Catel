# Build Catel Documentation with DocFx
param(
    [switch]$Clean,
    [switch]$Help
)

if ($Help) {
    Write-Host "DocFx Documentation Build Script"
    Write-Host ""
    Write-Host "Usage: .\build-docs.ps1 [options]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -Clean     Remove output directories before building"
    Write-Host "  -Help      Show this help message"
    exit 0
}

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$docfxPath = Join-Path $scriptPath "doc/docfx"
$docsPath = Join-Path $scriptPath "doc/site"

Write-Host "Catel Documentation Build Script"
Write-Host ""

if ($Clean -and (Test-Path $docsPath)) {
    Write-Host "Cleaning output directory: $docsPath"
    Remove-Item $docsPath -Recurse -Force
    Write-Host ""
}

Push-Location $docfxPath
docfx build docfx.json
Pop-Location

Write-Host ""
Write-Host "Build complete!"
Write-Host "Output: $docsPath"
