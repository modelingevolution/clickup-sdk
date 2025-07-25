using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelingEvolution.ClickUp;
using ModelingEvolution.ClickUp.Abstractions;
using System.Text;
using Xunit.Abstractions;
using Xunit.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Tests.IntegrationTests;

/// <summary>
/// Simple exploration test that respects rate limits
/// </summary>
public class ClickUpSimpleExplorationTest
{
    private readonly string? _apiToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ITestOutputHelper _output;
    private readonly IClickUpClient _client;

    public ClickUpSimpleExplorationTest(ITestOutputHelper output)
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
        
        _client = new ClickUpClientBuilder()
            .WithApiToken(_apiToken!)
            .WithLoggerFactory(_loggerFactory)
            .Build();
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task DiscoverBasicStructure_PrintsWorkspacesAndSpaces()
    {
        var report = new StringBuilder();
        report.AppendLine("=== ClickUp Basic Structure Discovery ===");
        report.AppendLine($"Discovery Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        report.AppendLine();

        var workspaces = new List<Models.Workspace>();
        
        try
        {
            // Get workspaces (rate limit: 100/min)
            _output.WriteLine("Fetching workspaces...");
            workspaces = (await _client.Workspaces.GetWorkspacesAsync()).ToList();
            report.AppendLine($"Found {workspaces.Count} workspace(s):");
            
            foreach (var workspace in workspaces)
            {
                report.AppendLine($"\n📁 Workspace: {workspace.Name}");
                report.AppendLine($"   ID: {workspace.Id}");
                report.AppendLine($"   Color: {workspace.Color ?? "none"}");
                
                // Small delay to respect rate limits
                await Task.Delay(1000);
                
                // Get spaces
                _output.WriteLine($"Fetching spaces for workspace {workspace.Name}...");
                var spaces = await _client.Spaces.GetSpacesAsync(workspace.Id);
                report.AppendLine($"\n   Found {spaces.Count} space(s):");
                
                foreach (var space in spaces)
                {
                    report.AppendLine($"\n   📂 Space: {space.Name}");
                    report.AppendLine($"      ID: {space.Id}");
                    report.AppendLine($"      Private: {space.Private}");
                    report.AppendLine($"      Archived: {space.Archived}");
                    
                    // For the first space, get some folders and lists as examples
                    if (space == spaces.First() && !space.Archived)
                    {
                        await Task.Delay(1000);
                        
                        _output.WriteLine($"Fetching folders for space {space.Name}...");
                        var folders = await _client.Folders.GetFoldersAsync(space.Id);
                        report.AppendLine($"\n      Sample - Found {folders.Count} folder(s) in this space");
                        
                        if (folders.Any())
                        {
                            var firstFolder = folders.First();
                            report.AppendLine($"      📁 Example Folder: {firstFolder.Name} (ID: {firstFolder.Id})");
                        }
                        
                        await Task.Delay(1000);
                        
                        _output.WriteLine($"Fetching folderless lists for space {space.Name}...");
                        var lists = await _client.Lists.GetFolderlessListsAsync(space.Id);
                        report.AppendLine($"      Found {lists.Count} folderless list(s) in this space");
                        
                        if (lists.Any())
                        {
                            var firstList = lists.First();
                            report.AppendLine($"      📋 Example List: {firstList.Name} (ID: {firstList.Id}, Tasks: {firstList.TaskCount ?? 0})");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            report.AppendLine($"\n❌ Error during discovery: {ex.Message}");
            _output.WriteLine($"Error: {ex}");
        }
        
        report.AppendLine("\n=== End of Basic Structure Discovery ===");
        
        var reportContent = report.ToString();
        _output.WriteLine(reportContent);
        
        // Write to file
        var fileName = $"clickup-basic-structure-{DateTime.UtcNow:yyyyMMdd-HHmmss}.txt";
        await File.WriteAllTextAsync(fileName, reportContent);
        _output.WriteLine($"\nReport written to: {fileName}");
        
        // Assertions
        Assert.True(workspaces.Any(), "Should have at least one workspace");
    }
}