# build.ps1 — Build and tag the Docker image from version.txt
#
# Usage:
#   .\build.ps1            # build only
#   .\build.ps1 -Push      # build + push to registry
#   .\build.ps1 -Up        # build + docker compose up

param(
    [switch]$Push,
    [switch]$Up
)

$ErrorActionPreference = 'Stop'

$version = (Get-Content "$PSScriptRoot\version.txt").Trim()
Write-Host "Building version: $version" -ForegroundColor Cyan

# Regenerate .env so docker-compose picks up the current version
"# Auto-generated from version.txt — do not edit manually." | Set-Content "$PSScriptRoot\.env"
"APP_VERSION=$version" | Add-Content "$PSScriptRoot\.env"

# Build image via docker compose (respects the Dockerfile layer cache)
docker compose `
    --file "$PSScriptRoot\docker-compose.yml" `
    build

if ($Push) {
    docker push "ventouxtina-web:$version"
}

if ($Up) {
    docker compose `
        --file "$PSScriptRoot\docker-compose.yml" `
        up -d
}

Write-Host "Done. Image tagged as ventouxtina-web:$version" -ForegroundColor Green

