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
/// Comprehensive integration tests to explore and document the ClickUp workspace structure.
/// These tests will discover all workspaces, spaces, folders, lists, and tasks.
/// </summary>
public class ClickUpExplorationTests
{
    private readonly string? _apiToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ITestOutputHelper _output;
    private readonly IClickUpClient _client;

    public ClickUpExplorationTests(ITestOutputHelper output)
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
    public async Task ExploreFullWorkspaceHierarchy_PrintsCompleteStructure()
    {
        var report = new StringBuilder();
        report.AppendLine("=== ClickUp Workspace Structure Discovery ===");
        report.AppendLine();

        // Get all workspaces
        var workspaces = await _client.Workspaces.GetWorkspacesAsync();
        report.AppendLine($"Found {workspaces.Count} workspace(s):");
        
        foreach (var workspace in workspaces)
        {
            report.AppendLine($"\n📁 Workspace: {workspace.Name} (ID: {workspace.Id})");
            report.AppendLine($"   Color: {workspace.Color ?? "none"}");
            // Workspace doesn't have members property in our model
            
            // Get spaces in this workspace
            var spaces = await _client.Spaces.GetSpacesAsync(workspace.Id);
            report.AppendLine($"\n   Found {spaces.Count} space(s) in workspace '{workspace.Name}':");
            
            foreach (var space in spaces)
            {
                report.AppendLine($"\n   📂 Space: {space.Name} (ID: {space.Id})");
                report.AppendLine($"      Private: {space.Private}");
                report.AppendLine($"      Color: {space.Color ?? "none"}");
                
                // Get folders in this space
                var folders = await _client.Folders.GetFoldersAsync(space.Id);
                report.AppendLine($"\n      Found {folders.Count} folder(s) in space '{space.Name}':");
                
                foreach (var folder in folders)
                {
                    report.AppendLine($"\n      📁 Folder: {folder.Name} (ID: {folder.Id})");
                    report.AppendLine($"         Hidden: {folder.Hidden}");
                    
                    // Get lists in this folder
                    var folderLists = await _client.Lists.GetListsAsync(folder.Id);
                    report.AppendLine($"         Found {folderLists.Count} list(s) in folder '{folder.Name}':");
                    
                    foreach (var list in folderLists)
                    {
                        report.AppendLine($"\n         📋 List: {list.Name} (ID: {list.Id})");
                        report.AppendLine($"            Status: {list.Status}");
                        report.AppendLine($"            Task Count: {list.TaskCount}");
                    }
                }
                
                // Get folderless lists
                var folderlessLists = await _client.Lists.GetFolderlessListsAsync(space.Id);
                if (folderlessLists.Count > 0)
                {
                    report.AppendLine($"\n      Found {folderlessLists.Count} folderless list(s) in space '{space.Name}':");
                    
                    foreach (var list in folderlessLists)
                    {
                        report.AppendLine($"\n      📋 List: {list.Name} (ID: {list.Id}) [No Folder]");
                        report.AppendLine($"         Status: {list.Status}");
                        report.AppendLine($"         Task Count: {list.TaskCount}");
                    }
                }
            }
        }
        
        report.AppendLine("\n=== End of Workspace Structure ===");
        
        var reportContent = report.ToString();
        _output.WriteLine(reportContent);
        
        // Also write to a file for easier viewing
        await File.WriteAllTextAsync("workspace-structure.txt", reportContent);
        
        // Assertions to ensure we found something
        workspaces.Should().NotBeEmpty("Should have at least one workspace");
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task GetTaskDetails_ForEachList_PrintsTaskInformation()
    {
        var report = new StringBuilder();
        report.AppendLine("=== ClickUp Task Discovery ===");
        report.AppendLine();

        var workspaces = await _client.Workspaces.GetWorkspacesAsync();
        
        foreach (var workspace in workspaces)
        {
            var spaces = await _client.Spaces.GetSpacesAsync(workspace.Id);
            
            foreach (var space in spaces)
            {
                report.AppendLine($"\n📂 Space: {space.Name}");
                
                // Get all lists (from folders and folderless)
                var allLists = new List<Models.List>();
                
                var folders = await _client.Folders.GetFoldersAsync(space.Id);
                foreach (var folder in folders)
                {
                    var folderLists = await _client.Lists.GetListsAsync(folder.Id);
                    allLists.AddRange(folderLists);
                }
                
                var folderlessLists = await _client.Lists.GetFolderlessListsAsync(space.Id);
                allLists.AddRange(folderlessLists);
                
                foreach (var list in allLists)
                {
                    report.AppendLine($"\n   📋 List: {list.Name}");
                    
                    try
                    {
                        var tasksResponse = await _client.Tasks.GetTasksAsync(list.Id);
                        report.AppendLine($"      Total Tasks: {tasksResponse.Tasks.Count}");
                        
                        // Show first 5 tasks
                        var tasksToShow = tasksResponse.Tasks.Take(5).ToList();
                        if (tasksToShow.Any())
                        {
                            report.AppendLine($"      First {tasksToShow.Count} tasks:");
                            foreach (var task in tasksToShow)
                            {
                                report.AppendLine($"         ✅ {task.Name} (ID: {task.Id})");
                                report.AppendLine($"            Status: {task.Status?.StatusName ?? "no status"}");
                                report.AppendLine($"            Priority: {task.Priority?.PriorityName ?? "no priority"}");
                                report.AppendLine($"            Created: {task.DateCreated}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        report.AppendLine($"      ❌ Error getting tasks: {ex.Message}");
                    }
                }
            }
        }
        
        report.AppendLine("\n=== End of Task Discovery ===");
        
        var reportContent = report.ToString();
        _output.WriteLine(reportContent);
        await File.WriteAllTextAsync("task-discovery.txt", reportContent);
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task GetWorkspaceStatistics_PrintsSummary()
    {
        var report = new StringBuilder();
        report.AppendLine("=== ClickUp Workspace Statistics ===");
        report.AppendLine();

        var totalSpaces = 0;
        var totalFolders = 0;
        var totalLists = 0;
        var totalTasks = 0;
        
        var workspaces = await _client.Workspaces.GetWorkspacesAsync();
        
        foreach (var workspace in workspaces)
        {
            var workspaceSpaces = 0;
            var workspaceFolders = 0;
            var workspaceLists = 0;
            var workspaceTasks = 0;
            
            report.AppendLine($"📊 Workspace: {workspace.Name}");
            
            var spaces = await _client.Spaces.GetSpacesAsync(workspace.Id);
            workspaceSpaces = spaces.Count;
            totalSpaces += workspaceSpaces;
            
            foreach (var space in spaces)
            {
                var folders = await _client.Folders.GetFoldersAsync(space.Id);
                workspaceFolders += folders.Count;
                totalFolders += folders.Count;
                
                // Count lists in folders
                foreach (var folder in folders)
                {
                    var lists = await _client.Lists.GetListsAsync(folder.Id);
                    workspaceLists += lists.Count;
                    totalLists += lists.Count;
                    
                    foreach (var list in lists)
                    {
                        workspaceTasks += list.TaskCount ?? 0;
                        totalTasks += list.TaskCount ?? 0;
                    }
                }
                
                // Count folderless lists
                var folderlessLists = await _client.Lists.GetFolderlessListsAsync(space.Id);
                workspaceLists += folderlessLists.Count;
                totalLists += folderlessLists.Count;
                
                foreach (var list in folderlessLists)
                {
                    workspaceTasks += list.TaskCount ?? 0;
                    totalTasks += list.TaskCount ?? 0;
                }
            }
            
            report.AppendLine($"   Spaces: {workspaceSpaces}");
            report.AppendLine($"   Folders: {workspaceFolders}");
            report.AppendLine($"   Lists: {workspaceLists}");
            report.AppendLine($"   Tasks: {workspaceTasks}");
            report.AppendLine();
        }
        
        report.AppendLine("📈 Total Statistics:");
        report.AppendLine($"   Workspaces: {workspaces.Count}");
        report.AppendLine($"   Spaces: {totalSpaces}");
        report.AppendLine($"   Folders: {totalFolders}");
        report.AppendLine($"   Lists: {totalLists}");
        report.AppendLine($"   Tasks: {totalTasks}");
        
        var reportContent = report.ToString();
        _output.WriteLine(reportContent);
        await File.WriteAllTextAsync("workspace-statistics.txt", reportContent);
        
        // Assertions
        workspaces.Should().NotBeEmpty();
        totalSpaces.Should().BeGreaterThan(0);
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task TestSpaceOperations_GetSpaceById()
    {
        // First get a workspace and space
        var workspaces = await _client.Workspaces.GetWorkspacesAsync();
        workspaces.Should().NotBeEmpty();
        
        var spaces = await _client.Spaces.GetSpacesAsync(workspaces.First().Id);
        spaces.Should().NotBeEmpty();
        
        var firstSpace = spaces.First();
        
        // Now test getting a specific space by ID
        var space = await _client.Spaces.GetSpaceAsync(firstSpace.Id);
        
        _output.WriteLine($"Got space by ID: {space.Name} (ID: {space.Id})");
        _output.WriteLine($"Private: {space.Private}");
        _output.WriteLine($"Archived: {space.Archived}");
        
        space.Should().NotBeNull();
        space.Id.Should().Be(firstSpace.Id);
        space.Name.Should().NotBeNullOrEmpty();
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task TestFolderOperations_GetFolderById()
    {
        // Navigate to find a folder
        var workspaces = await _client.Workspaces.GetWorkspacesAsync();
        var spaces = await _client.Spaces.GetSpacesAsync(workspaces.First().Id);
        
        Models.Folder? folderToTest = null;
        foreach (var space in spaces)
        {
            var folders = await _client.Folders.GetFoldersAsync(space.Id);
            if (folders.Any())
            {
                folderToTest = folders.First();
                break;
            }
        }
        
        if (folderToTest != null)
        {
            // Test getting folder by ID
            var folder = await _client.Folders.GetFolderAsync(folderToTest.Id);
            
            _output.WriteLine($"Got folder by ID: {folder.Name} (ID: {folder.Id})");
            _output.WriteLine($"Hidden: {folder.Hidden}");
            _output.WriteLine($"Task Count: {folder.TaskCount}");
            _output.WriteLine($"List Count: {folder.Lists?.Count ?? 0}");
            
            folder.Should().NotBeNull();
            folder.Id.Should().Be(folderToTest.Id);
            folder.Name.Should().NotBeNullOrEmpty();
        }
        else
        {
            _output.WriteLine("No folders found to test GetFolderById");
        }
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task TestListOperations_GetListById()
    {
        // Navigate to find a list
        var workspaces = await _client.Workspaces.GetWorkspacesAsync();
        var spaces = await _client.Spaces.GetSpacesAsync(workspaces.First().Id);
        
        Models.List? listToTest = null;
        foreach (var space in spaces)
        {
            // Try folderless lists first
            var folderlessLists = await _client.Lists.GetFolderlessListsAsync(space.Id);
            if (folderlessLists.Any())
            {
                listToTest = folderlessLists.First();
                break;
            }
            
            // Try lists in folders
            var folders = await _client.Folders.GetFoldersAsync(space.Id);
            foreach (var folder in folders)
            {
                var lists = await _client.Lists.GetListsAsync(folder.Id);
                if (lists.Any())
                {
                    listToTest = lists.First();
                    break;
                }
            }
            
            if (listToTest != null) break;
        }
        
        if (listToTest != null)
        {
            // Test getting list by ID
            var list = await _client.Lists.GetListAsync(listToTest.Id);
            
            _output.WriteLine($"Got list by ID: {list.Name} (ID: {list.Id})");
            _output.WriteLine($"Status: {list.Status}");
            _output.WriteLine($"Task Count: {list.TaskCount}");
            _output.WriteLine($"Archived: {list.Archived}");
            
            list.Should().NotBeNull();
            list.Id.Should().Be(listToTest.Id);
            list.Name.Should().NotBeNullOrEmpty();
        }
        else
        {
            _output.WriteLine("No lists found to test GetListById");
        }
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task TestTaskOperations_GetTaskById()
    {
        // Navigate to find tasks
        var workspaces = await _client.Workspaces.GetWorkspacesAsync();
        var spaces = await _client.Spaces.GetSpacesAsync(workspaces.First().Id);
        
        Models.TaskItem? taskToTest = null;
        foreach (var space in spaces)
        {
            var allLists = new List<Models.List>();
            
            // Get all lists
            var folders = await _client.Folders.GetFoldersAsync(space.Id);
            foreach (var folder in folders)
            {
                var lists = await _client.Lists.GetListsAsync(folder.Id);
                allLists.AddRange(lists);
            }
            allLists.AddRange(await _client.Lists.GetFolderlessListsAsync(space.Id));
            
            // Find a list with tasks
            foreach (var list in allLists.Where(l => l.TaskCount > 0))
            {
                var tasksResponse = await _client.Tasks.GetTasksAsync(list.Id);
                if (tasksResponse.Tasks.Any())
                {
                    taskToTest = tasksResponse.Tasks.First();
                    break;
                }
            }
            
            if (taskToTest != null) break;
        }
        
        if (taskToTest != null)
        {
            // Test getting task by ID
            var task = await _client.Tasks.GetTaskAsync(taskToTest.Id);
            
            _output.WriteLine($"Got task by ID: {task.Name} (ID: {task.Id})");
            _output.WriteLine($"Status: {task.Status?.StatusName ?? "no status"}");
            _output.WriteLine($"Priority: {task.Priority?.PriorityName ?? "no priority"}");
            _output.WriteLine($"Description: {task.Description ?? "no description"}");
            _output.WriteLine($"Created: {task.DateCreated}");
            _output.WriteLine($"Updated: {task.DateUpdated}");
            
            task.Should().NotBeNull();
            task.Id.Should().Be(taskToTest.Id);
            task.Name.Should().NotBeNullOrEmpty();
        }
        else
        {
            _output.WriteLine("No tasks found to test GetTaskById");
        }
    }
}