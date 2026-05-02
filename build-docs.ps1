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
$srcPath = Join-Path $scriptPath "doc/docfx"
$docsPath = Join-Path $scriptPath "doc/site"

Write-Host "Catel Documentation Build Script"
Write-Host ""

$versions = @("home", "vnext", "6.x")

foreach ($version in $versions) {
    $versionPath = Join-Path $srcPath $version
    $docfxPath = Join-Path $versionPath "docfx.json"
    
    if (-not (Test-Path $docfxPath)) {
        Write-Host "SKIP: $version (docfx.json not found)"
        continue
    }
    
    Write-Host "Building: $version"
    
    Push-Location $versionPath
    docfx build docfx.json
    Pop-Location
    
    Write-Host ""
}

Write-Host "Build complete!"
Write-Host "Output: $docsPath"
