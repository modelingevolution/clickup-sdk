using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface IListClient
{
    Task<IReadOnlyList<List>> GetListsAsync(string folderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<List>> GetFolderlessListsAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<List> GetListAsync(string listId, CancellationToken cancellationToken = default);
    Task<List> CreateListInFolderAsync(string folderId, CreateListRequest request, CancellationToken cancellationToken = default);
    Task<List> CreateListInSpaceAsync(string spaceId, CreateListRequest request, CancellationToken cancellationToken = default);
    Task<List> UpdateListAsync(string listId, UpdateListRequest request, CancellationToken cancellationToken = default);
    Task DeleteListAsync(string listId, CancellationToken cancellationToken = default);
}