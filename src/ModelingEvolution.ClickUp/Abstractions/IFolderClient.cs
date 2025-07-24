using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface IFolderClient
{
    Task<IReadOnlyList<Folder>> GetFoldersAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Folder> GetFolderAsync(string folderId, CancellationToken cancellationToken = default);
}