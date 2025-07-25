using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelingEvolution.ClickUp;
using Xunit.Abstractions;
using Xunit.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Tests.IntegrationTests;

/// <summary>
/// Integration tests that use real ClickUp API.
/// These tests are marked with [Fact(Skip = "Integration test")] by default.
/// To run them, you need to set the CLICKUP_API_TOKEN environment variable and remove the Skip attribute.
/// </summary>
public class ClickUpClientIntegrationTests
{
    private readonly string? _apiToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ITestOutputHelper _output;

    public ClickUpClientIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
            
        _apiToken = configuration["ClickUpToken"];
        _loggerFactory = LoggerFactory.Create(builder => 
        {
            builder.AddXunit(_output);
            builder.SetMinimumLevel(LogLevel.Debug);
        });
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task GetWorkspaces_ReturnsRealWorkspaces()
    {
        // Arrange
        var client = new ClickUpClientBuilder()
            .WithApiToken(_apiToken!)
            .WithLoggerFactory(_loggerFactory)
            .Build();

        // Act
        var workspaces = await client.Workspaces.GetWorkspacesAsync();

        // Assert
        workspaces.Should().NotBeNull();
        workspaces.Should().NotBeEmpty();
        workspaces.All(w => !string.IsNullOrEmpty(w.Id)).Should().BeTrue();
        workspaces.All(w => !string.IsNullOrEmpty(w.Name)).Should().BeTrue();
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task GetSpaces_ForFirstWorkspace_ReturnsSpaces()
    {
        // Arrange
        var client = new ClickUpClientBuilder()
            .WithApiToken(_apiToken!)
            .WithLoggerFactory(_loggerFactory)
            .Build();

        // Act
        var workspaces = await client.Workspaces.GetWorkspacesAsync();
        var firstWorkspace = workspaces.First();
        var spaces = await client.Spaces.GetSpacesAsync(firstWorkspace.Id);

        // Assert
        spaces.Should().NotBeNull();
        // Spaces might be empty, but the call should succeed
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task GetTasks_ForFirstList_ReturnsTasks()
    {
        // Arrange
        var client = new ClickUpClientBuilder()
            .WithApiToken(_apiToken!)
            .WithLoggerFactory(_loggerFactory)
            .Build();

        // Act - Navigate through hierarchy
        var workspaces = await client.Workspaces.GetWorkspacesAsync();
        if (!workspaces.Any()) return;

        var spaces = await client.Spaces.GetSpacesAsync(workspaces.First().Id);
        if (!spaces.Any()) return;

        var folders = await client.Folders.GetFoldersAsync(spaces.First().Id);
        
        // Try to get lists from folders or directly from space
        var lists = folders.Any() 
            ? await client.Lists.GetListsAsync(folders.First().Id)
            : await client.Lists.GetFolderlessListsAsync(spaces.First().Id);
            
        if (!lists.Any()) return;

        var tasksResponse = await client.Tasks.GetTasksAsync(lists.First().Id);

        // Assert
        tasksResponse.Should().NotBeNull();
        tasksResponse.Tasks.Should().NotBeNull();
    }
}