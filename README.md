# ModelingEvolution.ClickUp

A strongly-typed C# SDK for the ClickUp API, built with SOLID principles and modern .NET practices.

## Features

- Strongly typed models for all ClickUp entities
- Support for Workspaces, Spaces, Folders, Lists, and Tasks
- Token-based authentication
- Built on .NET 9.0
- Comprehensive unit tests
- Fluent builder pattern for client configuration
- Async/await throughout
- Proper logging support via Microsoft.Extensions.Logging

## Installation

```bash
# Clone the repository
git clone https://github.com/modelingevolution/clickup-csharp.git

# Build the project
dotnet build

# Run tests
dotnet test
```

## Quick Start

```csharp
using ModelingEvolution.ClickUp;
using Microsoft.Extensions.Logging;

// Create client using builder
var client = new ClickUpClientBuilder()
    .WithApiToken("your-api-token")
    .Build();

// Or with logging
var loggerFactory = LoggerFactory.Create(builder => 
{
    // Add your logging providers here
    // builder.AddConsole(); // requires Microsoft.Extensions.Logging.Console package
});
var clientWithLogging = new ClickUpClientBuilder()
    .WithApiToken("your-api-token")
    .WithLoggerFactory(loggerFactory)
    .Build();

// Get workspaces
var workspaces = await client.Workspaces.GetWorkspacesAsync();

// Get spaces for a workspace
var spaces = await client.Spaces.GetSpacesAsync(workspaces.First().Id);

// Get folders in a space
var folders = await client.Folders.GetFoldersAsync(spaces.First().Id);

// Get lists in a folder
var lists = await client.Lists.GetListsAsync(folders.First().Id);

// Get tasks in a list
var tasksResponse = await client.Tasks.GetTasksAsync(lists.First().Id);
var tasks = tasksResponse.Tasks;
```

## API Coverage

### Read Operations (Implemented)
- **Workspaces**: Get all workspaces
- **Spaces**: Get spaces by workspace, Get space by ID
- **Folders**: Get folders by space, Get folder by ID
- **Lists**: Get lists by folder, Get folderless lists, Get list by ID
- **Tasks**: Get tasks by list (with pagination), Get task by ID

### Write Operations (TODO)
- Create/Update/Delete operations for all entities
- Task status updates
- Comments and attachments
- Custom fields management

## Configuration

```csharp
var client = new ClickUpClientBuilder()
    .WithApiToken("your-api-token")
    .WithBaseUrl("https://api.clickup.com/api/v2") // Optional, this is default
    .WithTimeout(TimeSpan.FromSeconds(60))          // Optional, default is 30s
    .WithHttpClient(customHttpClient)                // Optional, provide your own HttpClient
    .WithLoggerFactory(loggerFactory)                // Optional, for logging
    .Build();
```

## Testing

The project includes comprehensive unit tests using xUnit, Moq, and FluentAssertions.

To run tests:
```bash
dotnet test
```

### Integration Tests

Integration tests are included but skipped by default. To run them:

1. Set the `CLICKUP_API_TOKEN` environment variable
2. Remove the `Skip` attribute from integration tests
3. Run tests normally

## Project Structure

```
src/
├── ModelingEvolution.ClickUp/
│   ├── Abstractions/      # Interfaces
│   ├── Clients/           # API client implementations
│   ├── Configuration/     # Configuration classes
│   ├── Http/              # HTTP client wrapper
│   └── Models/            # ClickUp data models
tests/
└── ModelingEvolution.ClickUp.Tests/
    ├── Clients/           # Unit tests for clients
    └── IntegrationTests/  # Integration tests
```

## Dependencies

- .NET 9.0
- System.Text.Json (9.0.7)
- Microsoft.Extensions.Http (9.0.7)
- Microsoft.Extensions.Logging.Abstractions (9.0.7)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## License

[Your License Here]

## Roadmap

- [ ] Write operations (Create, Update, Delete)
- [ ] Webhook support
- [ ] Rate limiting and retry logic
- [ ] Bulk operations
- [ ] Advanced search and filtering
- [ ] Time tracking API
- [ ] Goals API
- [ ] Custom fields API
- [ ] Attachment handling
- [ ] Comment management