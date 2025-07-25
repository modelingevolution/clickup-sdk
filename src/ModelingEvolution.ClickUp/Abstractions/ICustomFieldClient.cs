using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface ICustomFieldClient
{
    Task<IReadOnlyList<CustomFieldDefinition>> GetAccessibleCustomFieldsAsync(string listId, CancellationToken cancellationToken = default);
    Task SetCustomFieldValueAsync(string taskId, string fieldId, SetCustomFieldRequest request, CancellationToken cancellationToken = default);
    Task RemoveCustomFieldValueAsync(string taskId, string fieldId, CancellationToken cancellationToken = default);
}