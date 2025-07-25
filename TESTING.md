# ClickUp SDK Testing Guide

## Quick Start

### Running Tests

#### Bash (Linux/macOS/WSL)
```bash
# Run only unit tests
./test.sh

# Run both unit and integration tests
./test.sh --integration
# or
./test.sh -i
```

#### PowerShell (Windows)
```powershell
# Run only unit tests
./test.ps1

# Run both unit and integration tests
./test.ps1 -Integration
# or
./test.ps1 -i
```

## Test Structure

### Unit Tests
- Located in `tests/ModelingEvolution.ClickUp.Tests/`
- Run automatically in CI/CD pipelines
- Test individual components in isolation
- Use mocked dependencies

### Integration Tests
- Located in `tests/ModelingEvolution.ClickUp.Tests/IntegrationTests/`
- Disabled by default (marked with `[Fact(Skip = "...")]`)
- Require valid ClickUp API token in `appsettings.json`
- Test real API interactions

## Configuration

### API Token Setup
1. Create `tests/ModelingEvolution.ClickUp.Tests/appsettings.json`:
```json
{
  "ClickUpToken": "pk_YOUR_CLICKUP_API_TOKEN"
}
```

2. This file is already in `.gitignore` for security

### Manual Testing

To manually run specific tests:

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test --filter "FullyQualifiedName!~IntegrationTests"

# Run only integration tests (after manually removing Skip attributes)
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Run a specific test
dotnet test --filter "FullyQualifiedName~GetWorkspaces_ReturnsRealWorkspaces"
```

## Test Coverage

### Unit Tests (14 tests)
- ClickUpClientBuilder configuration
- HTTP client setup
- All client operations with mocked responses
- Error handling scenarios

### Integration Tests (3 tests)
- **GetWorkspaces_ReturnsRealWorkspaces**: Verifies workspace retrieval
- **GetSpaces_ForFirstWorkspace_ReturnsSpaces**: Tests space enumeration
- **GetTasks_ForFirstList_ReturnsTasks**: Validates task retrieval through full hierarchy

## Troubleshooting

### Integration Tests Failing

1. **404 Not Found**: Check if the API token is valid
2. **Timeout**: Verify network connectivity to ClickUp API
3. **Unauthorized**: Ensure the token has proper permissions

### Running Tests in VS Code

Add to `.vscode/tasks.json`:
```json
{
    "label": "test-integration",
    "command": "./test.sh",
    "args": ["--integration"],
    "type": "shell",
    "group": "test"
}
```