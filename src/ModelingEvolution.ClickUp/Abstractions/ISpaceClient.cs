using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface ISpaceClient
{
    Task<IReadOnlyList<Space>> GetSpacesAsync(string workspaceId, CancellationToken cancellationToken = default);
    Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default);
    Task<Space> CreateSpaceAsync(string workspaceId, CreateSpaceRequest request, CancellationToken cancellationToken = default);
    Task<Space> UpdateSpaceAsync(string spaceId, UpdateSpaceRequest request, CancellationToken cancellationToken = default);
    Task DeleteSpaceAsync(string spaceId, CancellationToken cancellationToken = default);
}