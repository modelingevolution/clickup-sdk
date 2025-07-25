using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface ITaskClient
{
    Task<TasksResponse> GetTasksAsync(string listId, int page = 0, CancellationToken cancellationToken = default);
    Task<TaskItem> GetTaskAsync(string taskId, CancellationToken cancellationToken = default);
    Task<TaskItem> CreateTaskAsync(string listId, CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskItem> UpdateTaskAsync(string taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(string taskId, CancellationToken cancellationToken = default);
}