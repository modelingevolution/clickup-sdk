using ModelingEvolution.ClickUp.Abstractions;
using ModelingEvolution.ClickUp.Http;
using ModelingEvolution.ClickUp.Models;
using Microsoft.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Clients;

public class CustomFieldClient : ICustomFieldClient
{
    private readonly ClickUpHttpClient _httpClient;
    private readonly ILogger<CustomFieldClient> _logger;

    public CustomFieldClient(ClickUpHttpClient httpClient, ILogger<CustomFieldClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<CustomFieldDefinition>> GetAccessibleCustomFieldsAsync(string listId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));
        
        _logger.LogInformation("Getting custom fields for list {ListId}", listId);
        
        var response = await _httpClient.GetAsync<CustomFieldsResponse>($"list/{listId}/field", cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} custom fields", response.Fields.Count);
        return response.Fields;
    }

    public async Task SetCustomFieldValueAsync(string taskId, string fieldId, SetCustomFieldRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty", nameof(taskId));
        
        if (string.IsNullOrWhiteSpace(fieldId))
            throw new ArgumentException("Field ID cannot be null or empty", nameof(fieldId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        _logger.LogInformation("Setting custom field {FieldId} value for task {TaskId}", fieldId, taskId);
        
        await _httpClient.PostAsync<object>($"task/{taskId}/field/{fieldId}", request, cancellationToken);
        
        _logger.LogInformation("Set custom field value successfully");
    }

    public async Task RemoveCustomFieldValueAsync(string taskId, string fieldId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty", nameof(taskId));
        
        if (string.IsNullOrWhiteSpace(fieldId))
            throw new ArgumentException("Field ID cannot be null or empty", nameof(fieldId));
        
        _logger.LogInformation("Removing custom field {FieldId} value from task {TaskId}", fieldId, taskId);
        
        await _httpClient.DeleteAsync($"task/{taskId}/field/{fieldId}", cancellationToken);
        
        _logger.LogInformation("Removed custom field value successfully");
    }
}