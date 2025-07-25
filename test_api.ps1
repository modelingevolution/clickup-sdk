$apiToken = "pk_4703678_EJE0ARRW88FOZSTUKBYDSEZL96GR6EJT"
$headers = @{
    "Authorization" = $apiToken
    "Content-Type" = "application/json"
}

Write-Host "Testing ClickUp API..."
Write-Host "Base URL: https://api.clickup.com/api/v2/"
Write-Host "Endpoint: team"
Write-Host "Full URL: https://api.clickup.com/api/v2/team"

try {
    $response = Invoke-RestMethod -Uri "https://api.clickup.com/api/v2/team" -Headers $headers -Method Get
    Write-Host "Success! Response:"
    $response | ConvertTo-Json
} catch {
    Write-Host "Error: $_"
}