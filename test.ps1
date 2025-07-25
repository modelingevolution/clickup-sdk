# ClickUp SDK Test Runner Script
# This script runs both unit tests and integration tests

param(
    [switch]$Integration,
    [Alias("i")]
    [switch]$I
)

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "ClickUp SDK Test Runner" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# Check if we're in the right directory
if (-not (Test-Path "ModelingEvolution.ClickUp.sln")) {
    Write-Host "Error: ModelingEvolution.ClickUp.sln not found!" -ForegroundColor Red
    Write-Host "Please run this script from the ClickUp SDK root directory."
    exit 1
}

# Function to run tests
function Run-Tests {
    param(
        [string]$TestType,
        [string]$Filter
    )
    
    Write-Host "`nRunning $TestType..." -ForegroundColor Yellow
    
    $result = dotnet test --filter $Filter --logger "console;verbosity=normal"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ $TestType passed!" -ForegroundColor Green
        return $true
    } else {
        Write-Host "✗ $TestType failed!" -ForegroundColor Red
        return $false
    }
}

# Build the solution first
Write-Host "`nBuilding solution..." -ForegroundColor Yellow
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Build successful!" -ForegroundColor Green

# Run unit tests
$unitTestResult = Run-Tests -TestType "Unit Tests" -Filter "FullyQualifiedName!~IntegrationTests"

# Check if integration tests should be run
$integrationTestResult = $true
if ($Integration -or $I) {
    Write-Host "`nEnabling integration tests..." -ForegroundColor Yellow
    
    # Create backup of the test file
    $testFile = "tests/ModelingEvolution.ClickUp.Tests/IntegrationTests/ClickUpClientIntegrationTests.cs"
    $backupFile = "$testFile.bak"
    
    Copy-Item $testFile $backupFile
    
    # Remove Skip attributes
    $content = Get-Content $testFile -Raw
    $content = $content -replace '\[Fact\(Skip = "Integration test - requires API token"\)\]', '[Fact]'
    Set-Content $testFile $content
    
    # Run integration tests
    $integrationTestResult = Run-Tests -TestType "Integration Tests" -Filter "FullyQualifiedName~IntegrationTests"
    
    # Restore the original file
    Move-Item $backupFile $testFile -Force
    
    Write-Host "`nIntegration tests have been re-disabled." -ForegroundColor Yellow
} else {
    Write-Host "`nSkipping integration tests. Use -Integration or -i to run them." -ForegroundColor Yellow
}

# Summary
Write-Host "`n=========================================" -ForegroundColor Cyan
Write-Host "Test Summary:" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

if ($unitTestResult) {
    Write-Host "✓ Unit Tests: PASSED" -ForegroundColor Green
} else {
    Write-Host "✗ Unit Tests: FAILED" -ForegroundColor Red
}

if ($Integration -or $I) {
    if ($integrationTestResult) {
        Write-Host "✓ Integration Tests: PASSED" -ForegroundColor Green
    } else {
        Write-Host "✗ Integration Tests: FAILED" -ForegroundColor Red
    }
}

Write-Host "=========================================" -ForegroundColor Cyan

# Exit with error if any tests failed
if (-not $unitTestResult -or -not $integrationTestResult) {
    exit 1
}

exit 0