using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface ISpaceClient
{
    Task<IReadOnlyList<Space>> GetSpacesAsync(string workspaceId, CancellationToken cancellationToken = default);
    Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default);
}