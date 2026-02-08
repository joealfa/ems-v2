# RabbitMQ Setup Script for EMS-v2
# Creates the virtual host, exchange, queue, and bindings required by the application

$rabbitHost = "localhost"
$rabbitPort = "15672"
$rabbitUser = "admin"
$rabbitPassword = Read-Host "Enter RabbitMQ password" -AsSecureString
$plainPassword = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($rabbitPassword))
$vhost = "ems"

# Base64 encode credentials for Basic Auth
$base64Auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("${rabbitUser}:${plainPassword}"))
$headers = @{
    Authorization = "Basic $base64Auth"
    "Content-Type" = "application/json"
}

$baseUrl = "http://${rabbitHost}:${rabbitPort}/api"
$vhostEncoded = [System.Web.HttpUtility]::UrlEncode($vhost)

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "EMS-v2 RabbitMQ Setup" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# 1. Create virtual host
Write-Host "Creating virtual host: $vhost"
try {
    Invoke-RestMethod -Uri "$baseUrl/vhosts/$vhostEncoded" `
        -Method Put `
        -Headers $headers
    Write-Host "  [OK] Virtual host created" -ForegroundColor Green
} catch {
    Write-Host "  [FAIL] Failed to create virtual host: $($_.Exception.Message)" -ForegroundColor Red
}

# 2. Set permissions for user on virtual host
Write-Host "`nSetting permissions for '$rabbitUser' on vhost '$vhost'"
$permBody = @{
    configure = ".*"
    write = ".*"
    read = ".*"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "$baseUrl/permissions/$vhostEncoded/$rabbitUser" `
        -Method Put `
        -Headers $headers `
        -Body $permBody
    Write-Host "  [OK] Permissions set" -ForegroundColor Green
} catch {
    Write-Host "  [FAIL] Failed to set permissions: $($_.Exception.Message)" -ForegroundColor Red
}

# 3. Create topic exchange: ems.events
$exchangeName = "ems.events"
$exchangeBody = @{
    type = "topic"
    durable = $true
    auto_delete = $false
} | ConvertTo-Json

Write-Host "`nCreating exchange: $exchangeName (topic, durable)"
try {
    Invoke-RestMethod -Uri "$baseUrl/exchanges/$vhostEncoded/$exchangeName" `
        -Method Put `
        -Headers $headers `
        -Body $exchangeBody
    Write-Host "  [OK] Exchange created" -ForegroundColor Green
} catch {
    Write-Host "  [FAIL] Failed to create exchange: $($_.Exception.Message)" -ForegroundColor Red
}

# 4. Create gateway cache-invalidation queue
$queueName = "ems.gateway.cache-invalidation"
$queueBody = @{
    durable = $true
    auto_delete = $false
    arguments = @{
        "x-message-ttl" = 86400000   # 24 hours
        "x-max-length" = 10000
    }
} | ConvertTo-Json

Write-Host "`nCreating queue: $queueName"
try {
    Invoke-RestMethod -Uri "$baseUrl/queues/$vhostEncoded/$queueName" `
        -Method Put `
        -Headers $headers `
        -Body $queueBody
    Write-Host "  [OK] Queue created" -ForegroundColor Green
} catch {
    Write-Host "  [FAIL] Failed to create queue: $($_.Exception.Message)" -ForegroundColor Red
}

# 5. Bind queue to exchange with routing key com.ems.#
$routingKey = "com.ems.#"
$bindingBody = @{
    routing_key = $routingKey
    arguments = @{}
} | ConvertTo-Json

Write-Host "`nBinding queue '$queueName' to exchange '$exchangeName' (routing key: $routingKey)"
try {
    Invoke-RestMethod -Uri "$baseUrl/bindings/$vhostEncoded/e/$exchangeName/q/$queueName" `
        -Method Post `
        -Headers $headers `
        -Body $bindingBody
    Write-Host "  [OK] Binding created" -ForegroundColor Green
} catch {
    Write-Host "  [FAIL] Failed to create binding: $($_.Exception.Message)" -ForegroundColor Red
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nResources created:"
Write-Host "  Virtual Host : $vhost"
Write-Host "  Exchange     : $exchangeName (topic)"
Write-Host "  Queue        : $queueName"
Write-Host "  Binding      : $exchangeName -> $queueName (routing key: $routingKey)"
Write-Host "`nManagement UI: http://${rabbitHost}:${rabbitPort}"
Write-Host ""
