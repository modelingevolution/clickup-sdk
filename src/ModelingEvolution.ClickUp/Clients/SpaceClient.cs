using ModelingEvolution.ClickUp.Abstractions;
using ModelingEvolution.ClickUp.Http;
using ModelingEvolution.ClickUp.Models;
using Microsoft.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Clients;

public class SpaceClient : ISpaceClient
{
    private readonly ClickUpHttpClient _httpClient;
    private readonly ILogger<SpaceClient> _logger;

    public SpaceClient(ClickUpHttpClient httpClient, ILogger<SpaceClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<Space>> GetSpacesAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workspaceId))
            throw new ArgumentException("Workspace ID cannot be null or empty", nameof(workspaceId));
        
        _logger.LogInformation("Getting spaces for workspace {WorkspaceId}", workspaceId);
        
        var response = await _httpClient.GetAsync<SpacesResponse>($"/team/{workspaceId}/space", cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} spaces", response.Spaces.Count);
        return response.Spaces;
    }

    public async Task<Space> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(spaceId))
            throw new ArgumentException("Space ID cannot be null or empty", nameof(spaceId));
        
        _logger.LogInformation("Getting space {SpaceId}", spaceId);
        
        var space = await _httpClient.GetAsync<Space>($"/space/{spaceId}", cancellationToken);
        
        _logger.LogInformation("Retrieved space: {SpaceName}", space.Name);
        return space;
    }
}