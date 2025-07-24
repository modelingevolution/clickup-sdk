using ModelingEvolution.ClickUp.Abstractions;
using ModelingEvolution.ClickUp.Http;
using ModelingEvolution.ClickUp.Models;
using Microsoft.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Clients;

public class WorkspaceClient : IWorkspaceClient
{
    private readonly ClickUpHttpClient _httpClient;
    private readonly ILogger<WorkspaceClient> _logger;

    public WorkspaceClient(ClickUpHttpClient httpClient, ILogger<WorkspaceClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<Workspace>> GetWorkspacesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting workspaces");
        
        var response = await _httpClient.GetAsync<WorkspacesResponse>("team", cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} workspaces", response.Teams.Count);
        return response.Teams;
    }
}