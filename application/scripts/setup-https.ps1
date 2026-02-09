# PowerShell script to export ASP.NET Core development certificate for Vite

Write-Host "Exporting ASP.NET Core Development Certificate for Vite..." -ForegroundColor Green

# Create certs directory if it doesn't exist
$certsDir = Join-Path $PSScriptRoot "certs"
if (-not (Test-Path $certsDir)) {
    New-Item -ItemType Directory -Path $certsDir | Out-Null
}

# Export the certificate to PFX format first
$pfxPath = Join-Path $certsDir "localhost.pfx"
$pemCertPath = Join-Path $certsDir "localhost.pem"
$pemKeyPath = Join-Path $certsDir "localhost-key.pem"

# Password for the PFX file (temporary, just for export)
$password = "temp-password-for-export"
$securePassword = ConvertTo-SecureString -String $password -Force -AsPlainText

Write-Host "Exporting certificate to PFX..." -ForegroundColor Yellow

# Export the ASP.NET Core development certificate
try {
    # Find the ASP.NET Core development certificate (get the most recent valid one)
    $cert = Get-ChildItem Cert:\CurrentUser\My | Where-Object {
        $_.Subject -like "CN=localhost" -and 
        $_.Issuer -like "CN=localhost" -and
        $_.HasPrivateKey -eq $true -and
        $_.NotAfter -gt (Get-Date)
    } | Sort-Object -Property NotBefore -Descending | Select-Object -First 1

    if ($null -eq $cert) {
        Write-Host "Error: ASP.NET Core development certificate not found." -ForegroundColor Red
        Write-Host "Please run: dotnet dev-certs https --trust" -ForegroundColor Yellow
        exit 1
    }

    Write-Host "Found certificate: $($cert.Subject)" -ForegroundColor Green
    Write-Host "Thumbprint: $($cert.Thumbprint)" -ForegroundColor Gray
    Write-Host "Expires: $($cert.NotAfter)" -ForegroundColor Gray

    # Export to PFX
    Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $securePassword | Out-Null
    Write-Host "Certificate exported to PFX" -ForegroundColor Green

    # Convert PFX to PEM format using OpenSSL (if available)
    $opensslPath = Get-Command openssl -ErrorAction SilentlyContinue
    
    # Check common OpenSSL installation paths
    $opensslExe = $null
    if ($opensslPath) {
        $opensslExe = $opensslPath.Path
    } elseif (Test-Path "C:\Program Files\OpenSSL-Win64\bin\openssl.exe") {
        $opensslExe = "C:\Program Files\OpenSSL-Win64\bin\openssl.exe"
    }
    
    if ($opensslExe) {
        Write-Host "Converting to PEM format using OpenSSL..." -ForegroundColor Yellow
        Write-Host "Using: $opensslExe" -ForegroundColor Gray
        
        # Extract certificate
        & $opensslExe pkcs12 -in $pfxPath -clcerts -nokeys -out $pemCertPath -password pass:$password -passin pass:$password 2>$null
        
        # Extract private key
        & $opensslExe pkcs12 -in $pfxPath -nocerts -nodes -out $pemKeyPath -password pass:$password -passin pass:$password 2>$null
        
        Write-Host "Certificate and key exported to PEM format" -ForegroundColor Green
        Write-Host "  Certificate: $pemCertPath" -ForegroundColor Cyan
        Write-Host "  Private Key: $pemKeyPath" -ForegroundColor Cyan
        
        # Clean up PFX file
        Remove-Item $pfxPath -Force
        
    } else {
        Write-Host "OpenSSL not found. Keeping PFX file: $pfxPath" -ForegroundColor Yellow
        Write-Host "You can convert it manually or install OpenSSL from: https://slproweb.com/products/Win32OpenSSL.html" -ForegroundColor Yellow
        Write-Host "Then run this script again." -ForegroundColor Yellow
        
        # We'll use an alternative approach - install a package to handle this
        Write-Host ""
        Write-Host "Alternative: Install @vitejs/plugin-basic-ssl" -ForegroundColor Cyan
        Write-Host "Run: npm install -D @vitejs/plugin-basic-ssl" -ForegroundColor White
        exit 1
    }

    Write-Host ""
    Write-Host "✓ Setup complete! Your certificates are ready." -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. The vite.config.ts will be updated to use these certificates" -ForegroundColor White
    Write-Host "2. Update your .env file: http://localhost:5173 → https://localhost:5173" -ForegroundColor White
    Write-Host "3. Update Google OAuth authorized origins to include https://localhost:5173" -ForegroundColor White
    Write-Host ""
    
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
