using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface IListClient
{
    Task<IReadOnlyList<List>> GetListsAsync(string folderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<List>> GetFolderlessListsAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<List> GetListAsync(string listId, CancellationToken cancellationToken = default);
}