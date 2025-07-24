using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface ITaskClient
{
    Task<TasksResponse> GetTasksAsync(string listId, int page = 0, CancellationToken cancellationToken = default);
    Task<TaskItem> GetTaskAsync(string taskId, CancellationToken cancellationToken = default);
}