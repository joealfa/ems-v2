# generate-api-client.ps1
# Generates the NSwag API client from the running EMS API's OpenAPI specification

param(
    [string]$ApiUrl = "https://localhost:7166",
    [string]$ConfigFile = "EmployeeManagementSystem.ApiClient/nswag.json",
    [switch]$SkipCertificateCheck
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$serverDir = Split-Path -Parent $scriptDir

Push-Location $serverDir

try {
    Write-Host "Generating API client using NSwag..." -ForegroundColor Cyan
    Write-Host "API URL: $ApiUrl" -ForegroundColor Cyan

    # Check if the nswag.json file exists
    $configPath = Join-Path $serverDir $ConfigFile
    if (-not (Test-Path $configPath)) {
        throw "NSwag configuration file not found: $configPath"
    }

    Write-Host "Testing API connectivity..." -ForegroundColor Cyan
    try {
        $webRequestParams = @{
            Uri = "$ApiUrl/openapi/v1.json"
            Method = "Get"
            TimeoutSec = 10
        }

        # For development with self-signed certificates
        if ($SkipCertificateCheck -or $ApiUrl -like "*localhost*") {
            $webRequestParams.SkipCertificateCheck = $true
        }

        $response = Invoke-WebRequest @webRequestParams
        if ($response.StatusCode -eq 200) {
            Write-Host "API is reachable!" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "Warning: Could not reach API at $ApiUrl/openapi/v1.json" -ForegroundColor Yellow
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Yellow
        Write-Host "Make sure the API server is running before generating the client." -ForegroundColor Yellow
        throw "API server is not reachable. Please start the API server first."
    }

    # Navigate to the ApiClient directory
    $apiClientDir = Join-Path $serverDir "EmployeeManagementSystem.ApiClient"
    Push-Location $apiClientDir

    try {
        # Ensure the Generated directory exists
        $generatedDir = Join-Path $apiClientDir "Generated"
        if (-not (Test-Path $generatedDir)) {
            New-Item -ItemType Directory -Path $generatedDir -Force | Out-Null
            Write-Host "Created Generated directory" -ForegroundColor Green
        }

        nswag run nswag.json

        if ($LASTEXITCODE -ne 0) {
            throw "NSwag generation failed with exit code $LASTEXITCODE"
        }

        Write-Host "`nAPI client generated successfully!" -ForegroundColor Green
        Write-Host "Generated file: Generated/EmsApiClient.cs" -ForegroundColor Green
    }
    finally {
        Pop-Location
    }
}
finally {
    Pop-Location
}
