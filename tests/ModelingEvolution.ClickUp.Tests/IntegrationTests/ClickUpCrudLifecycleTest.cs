using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelingEvolution.ClickUp;
using ModelingEvolution.ClickUp.Abstractions;
using ModelingEvolution.ClickUp.Models;
using System.Text;
using Xunit.Abstractions;
using Xunit.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Tests.IntegrationTests;

/// <summary>
/// Comprehensive CRUD lifecycle test for ClickUp SDK.
/// Creates a test space, folder, list, tasks with custom fields, then cleans up everything.
/// </summary>
public class ClickUpCrudLifecycleTest : IAsyncLifetime
{
    private readonly string? _apiToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ITestOutputHelper _output;
    private readonly IClickUpClient _client;
    private readonly string _testNameSuffix;
    
    // Track created resources for cleanup
    private string? _createdSpaceId;
    private string? _createdFolderId;
    private string? _createdListId;
    private readonly List<string> _createdTaskIds = new();

    public ClickUpCrudLifecycleTest(ITestOutputHelper output)
    {
        _output = output;
        _testNameSuffix = $"Test_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
        
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

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        // Cleanup any resources that might have been created
        await CleanupTestResources();
    }

    [Fact(Skip = "Integration test - requires API token")]
    public async Task FullCrudLifecycle_CreateUpdateDelete_Success()
    {
        var report = new StringBuilder();
        report.AppendLine($"=== ClickUp CRUD Lifecycle Test ===");
        report.AppendLine($"Test Run: {_testNameSuffix}");
        report.AppendLine($"Started: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        report.AppendLine();

        try
        {
            // Step 1: Get workspace (we can't create workspaces via API)
            _output.WriteLine("Step 1: Getting workspace...");
            var workspaces = await _client.Workspaces.GetWorkspacesAsync();
            workspaces.Should().NotBeEmpty("Need at least one workspace");
            var workspace = workspaces.First();
            report.AppendLine($"Using workspace: {workspace.Name} (ID: {workspace.Id})");
            
            // Add delay to respect rate limits
            await Task.Delay(1000);
            
            // Step 2: Create a test space
            _output.WriteLine("Step 2: Creating test space...");
            var createSpaceRequest = new CreateSpaceRequest
            {
                Name = $"SDK_Test_Space_{_testNameSuffix}",
                MultipleAssignees = true,
                Features = new SpaceFeatures
                {
                    DueDates = new FeatureSetting { Enabled = true },
                    TimeEstimates = new FeatureSetting { Enabled = true },
                    CustomFields = new FeatureSetting { Enabled = true }
                }
            };
            
            var space = await _client.Spaces.CreateSpaceAsync(workspace.Id, createSpaceRequest);
            _createdSpaceId = space.Id;
            report.AppendLine($"Created space: {space.Name} (ID: {space.Id})");
            
            await Task.Delay(1000);
            
            // Step 3: Create a folder in the space
            _output.WriteLine("Step 3: Creating test folder...");
            var createFolderRequest = new CreateFolderRequest
            {
                Name = $"Test_Folder_{DateTime.UtcNow:HHmmss}"
            };
            
            var folder = await _client.Folders.CreateFolderAsync(space.Id, createFolderRequest);
            _createdFolderId = folder.Id;
            report.AppendLine($"Created folder: {folder.Name} (ID: {folder.Id})");
            
            await Task.Delay(1000);
            
            // Step 4: Create a list in the folder
            _output.WriteLine("Step 4: Creating test list...");
            var createListRequest = new CreateListRequest
            {
                Name = $"Test_List_{DateTime.UtcNow:HHmmss}",
                Content = "This is a test list created by SDK integration tests"
            };
            
            var list = await _client.Lists.CreateListInFolderAsync(folder.Id, createListRequest);
            _createdListId = list.Id;
            report.AppendLine($"Created list: {list.Name} (ID: {list.Id})");
            
            await Task.Delay(1000);
            
            // Step 5: Get custom fields for the list
            _output.WriteLine("Step 5: Getting custom fields...");
            var customFields = await _client.CustomFields.GetAccessibleCustomFieldsAsync(list.Id);
            report.AppendLine($"Found {customFields.Count} custom fields");
            
            await Task.Delay(1000);
            
            // Step 6: Create tasks
            _output.WriteLine("Step 6: Creating test tasks...");
            var taskNames = new[] { "Task Alpha", "Task Beta", "Task Gamma" };
            foreach (var taskName in taskNames)
            {
                var createTaskRequest = new CreateTaskRequest
                {
                    Name = $"{taskName} - {_testNameSuffix}",
                    Description = $"Test task created at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC",
                    Priority = 2,
                    Tags = new[] { "sdk-test", "automated" }
                };
                
                var task = await _client.Tasks.CreateTaskAsync(list.Id, createTaskRequest);
                _createdTaskIds.Add(task.Id);
                report.AppendLine($"Created task: {task.Name} (ID: {task.Id})");
                
                await Task.Delay(500);
            }
            
            await Task.Delay(1000);
            
            // Step 7: Update a task
            _output.WriteLine("Step 7: Updating a task...");
            if (_createdTaskIds.Any())
            {
                var taskToUpdate = _createdTaskIds.First();
                var updateTaskRequest = new UpdateTaskRequest
                {
                    Name = $"Updated Task - {DateTime.UtcNow:HH:mm:ss}",
                    Description = $"This task was updated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC",
                    Priority = 1
                };
                
                var updatedTask = await _client.Tasks.UpdateTaskAsync(taskToUpdate, updateTaskRequest);
                report.AppendLine($"Updated task: {updatedTask.Name}");
                
                await Task.Delay(1000);
            }
            
            // Step 8: Set custom field value (if available)
            _output.WriteLine("Step 8: Setting custom field value...");
            if (customFields.Any() && _createdTaskIds.Any())
            {
                var textField = customFields.FirstOrDefault(f => f.Type == "text" || f.Type == "short_text");
                if (textField != null)
                {
                    var setFieldRequest = new SetCustomFieldRequest
                    {
                        Value = $"SDK Test Value - {DateTime.UtcNow:HH:mm:ss}"
                    };
                    
                    await _client.CustomFields.SetCustomFieldValueAsync(_createdTaskIds.First(), textField.Id, setFieldRequest);
                    report.AppendLine($"Set custom field '{textField.Name}' value");
                    
                    await Task.Delay(1000);
                }
            }
            
            // Step 9: Update folder name
            _output.WriteLine("Step 9: Updating folder...");
            var updateFolderRequest = new UpdateFolderRequest
            {
                Name = $"Updated_Folder_{DateTime.UtcNow:HHmmss}"
            };
            
            var updatedFolder = await _client.Folders.UpdateFolderAsync(folder.Id, updateFolderRequest);
            report.AppendLine($"Updated folder: {updatedFolder.Name}");
            
            await Task.Delay(1000);
            
            // Step 10: Update list
            _output.WriteLine("Step 10: Updating list...");
            var updateListRequest = new UpdateListRequest
            {
                Name = $"Updated_List_{DateTime.UtcNow:HHmmss}",
                Content = "This list was updated by SDK integration tests"
            };
            
            var updatedList = await _client.Lists.UpdateListAsync(list.Id, updateListRequest);
            report.AppendLine($"Updated list: {updatedList.Name}");
            
            await Task.Delay(1000);
            
            // Step 11: Delete a task
            _output.WriteLine("Step 11: Deleting a task...");
            if (_createdTaskIds.Count > 1)
            {
                var taskToDelete = _createdTaskIds.Last();
                await _client.Tasks.DeleteTaskAsync(taskToDelete);
                _createdTaskIds.Remove(taskToDelete);
                report.AppendLine($"Deleted task ID: {taskToDelete}");
                
                await Task.Delay(1000);
            }
            
            // Step 12: Clean up - Delete remaining tasks
            _output.WriteLine("Step 12: Cleaning up tasks...");
            foreach (var taskId in _createdTaskIds.ToList())
            {
                await _client.Tasks.DeleteTaskAsync(taskId);
                report.AppendLine($"Deleted task ID: {taskId}");
                await Task.Delay(500);
            }
            _createdTaskIds.Clear();
            
            await Task.Delay(1000);
            
            // Step 13: Delete list
            _output.WriteLine("Step 13: Deleting list...");
            await _client.Lists.DeleteListAsync(list.Id);
            _createdListId = null;
            report.AppendLine($"Deleted list ID: {list.Id}");
            
            await Task.Delay(1000);
            
            // Step 14: Delete folder
            _output.WriteLine("Step 14: Deleting folder...");
            await _client.Folders.DeleteFolderAsync(folder.Id);
            _createdFolderId = null;
            report.AppendLine($"Deleted folder ID: {folder.Id}");
            
            await Task.Delay(1000);
            
            // Step 15: Delete space
            _output.WriteLine("Step 15: Deleting space...");
            await _client.Spaces.DeleteSpaceAsync(space.Id);
            _createdSpaceId = null;
            report.AppendLine($"Deleted space ID: {space.Id}");
            
            report.AppendLine();
            report.AppendLine("✅ All CRUD operations completed successfully!");
            
        }
        catch (Exception ex)
        {
            report.AppendLine();
            report.AppendLine($"❌ Test failed with error: {ex.Message}");
            report.AppendLine($"Stack trace: {ex.StackTrace}");
            
            // Attempt cleanup
            _output.WriteLine("Attempting cleanup after error...");
            await CleanupTestResources();
            
            throw;
        }
        finally
        {
            report.AppendLine();
            report.AppendLine($"Test completed: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            report.AppendLine("=== End of CRUD Lifecycle Test ===");
            
            var reportContent = report.ToString();
            _output.WriteLine(reportContent);
            
            // Write report to file
            var fileName = $"clickup-crud-test-{_testNameSuffix}.txt";
            await File.WriteAllTextAsync(fileName, reportContent);
            _output.WriteLine($"\nReport written to: {fileName}");
        }
    }
    
    private async Task CleanupTestResources()
    {
        try
        {
            // Delete tasks
            foreach (var taskId in _createdTaskIds.ToList())
            {
                try
                {
                    await _client.Tasks.DeleteTaskAsync(taskId);
                    _output.WriteLine($"Cleaned up task: {taskId}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Failed to delete task {taskId}: {ex.Message}");
                }
                await Task.Delay(500);
            }
            
            // Delete list
            if (!string.IsNullOrEmpty(_createdListId))
            {
                try
                {
                    await _client.Lists.DeleteListAsync(_createdListId);
                    _output.WriteLine($"Cleaned up list: {_createdListId}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Failed to delete list {_createdListId}: {ex.Message}");
                }
                await Task.Delay(1000);
            }
            
            // Delete folder
            if (!string.IsNullOrEmpty(_createdFolderId))
            {
                try
                {
                    await _client.Folders.DeleteFolderAsync(_createdFolderId);
                    _output.WriteLine($"Cleaned up folder: {_createdFolderId}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Failed to delete folder {_createdFolderId}: {ex.Message}");
                }
                await Task.Delay(1000);
            }
            
            // Delete space
            if (!string.IsNullOrEmpty(_createdSpaceId))
            {
                try
                {
                    await _client.Spaces.DeleteSpaceAsync(_createdSpaceId);
                    _output.WriteLine($"Cleaned up space: {_createdSpaceId}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Failed to delete space {_createdSpaceId}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Cleanup error: {ex.Message}");
        }
    }
}