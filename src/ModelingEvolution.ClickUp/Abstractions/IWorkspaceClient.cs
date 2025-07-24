using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface IWorkspaceClient
{
    Task<IReadOnlyList<Workspace>> GetWorkspacesAsync(CancellationToken cancellationToken = default);
}