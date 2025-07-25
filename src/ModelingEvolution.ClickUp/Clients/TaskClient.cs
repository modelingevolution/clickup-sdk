using ModelingEvolution.ClickUp.Abstractions;
using ModelingEvolution.ClickUp.Http;
using ModelingEvolution.ClickUp.Models;
using Microsoft.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Clients;

public class TaskClient : ITaskClient
{
    private readonly ClickUpHttpClient _httpClient;
    private readonly ILogger<TaskClient> _logger;

    public TaskClient(ClickUpHttpClient httpClient, ILogger<TaskClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TasksResponse> GetTasksAsync(string listId, int page = 0, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));
        
        _logger.LogInformation("Getting tasks for list {ListId}, page {Page}", listId, page);
        
        var response = await _httpClient.GetAsync<TasksResponse>($"list/{listId}/task?page={page}", cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} tasks", response.Tasks.Count);
        return response;
    }

    public async Task<TaskItem> GetTaskAsync(string taskId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty", nameof(taskId));
        
        _logger.LogInformation("Getting task {TaskId}", taskId);
        
        var task = await _httpClient.GetAsync<TaskItem>($"task/{taskId}", cancellationToken);
        
        _logger.LogInformation("Retrieved task: {TaskName}", task.Name);
        return task;
    }
    
    public async Task<TaskItem> CreateTaskAsync(string listId, CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        _logger.LogInformation("Creating task '{TaskName}' in list {ListId}", request.Name, listId);
        
        var task = await _httpClient.PostAsync<TaskItem>($"list/{listId}/task", request, cancellationToken);
        
        _logger.LogInformation("Created task: {TaskName} (ID: {TaskId})", task.Name, task.Id);
        return task;
    }
    
    public async Task<TaskItem> UpdateTaskAsync(string taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty", nameof(taskId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        _logger.LogInformation("Updating task {TaskId}", taskId);
        
        var task = await _httpClient.PutAsync<TaskItem>($"task/{taskId}", request, cancellationToken);
        
        _logger.LogInformation("Updated task: {TaskName}", task.Name);
        return task;
    }
    
    public async Task DeleteTaskAsync(string taskId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty", nameof(taskId));
        
        _logger.LogInformation("Deleting task {TaskId}", taskId);
        
        await _httpClient.DeleteAsync($"task/{taskId}", cancellationToken);
        
        _logger.LogInformation("Deleted task {TaskId}", taskId);
    }
}