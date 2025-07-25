using ModelingEvolution.ClickUp.Models;

namespace ModelingEvolution.ClickUp.Abstractions;

public interface IWorkspaceClient
{
    Task<IReadOnlyList<Workspace>> GetWorkspacesAsync(CancellationToken cancellationToken = default);
    
    // Note: ClickUp API doesn't support creating/deleting workspaces via API
    // Workspaces are managed through the ClickUp UI only
}