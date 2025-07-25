using ModelingEvolution.ClickUp.Abstractions;
using ModelingEvolution.ClickUp.Http;
using ModelingEvolution.ClickUp.Models;
using Microsoft.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Clients;

public class ListClient : IListClient
{
    private readonly ClickUpHttpClient _httpClient;
    private readonly ILogger<ListClient> _logger;

    public ListClient(ClickUpHttpClient httpClient, ILogger<ListClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<List>> GetListsAsync(string folderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folderId))
            throw new ArgumentException("Folder ID cannot be null or empty", nameof(folderId));
        
        _logger.LogInformation("Getting lists for folder {FolderId}", folderId);
        
        var response = await _httpClient.GetAsync<ListsResponse>($"folder/{folderId}/list", cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} lists", response.Lists.Count);
        return response.Lists;
    }

    public async Task<IReadOnlyList<List>> GetFolderlessListsAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(spaceId))
            throw new ArgumentException("Space ID cannot be null or empty", nameof(spaceId));
        
        _logger.LogInformation("Getting folderless lists for space {SpaceId}", spaceId);
        
        var response = await _httpClient.GetAsync<ListsResponse>($"space/{spaceId}/list", cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} folderless lists", response.Lists.Count);
        return response.Lists;
    }

    public async Task<List> GetListAsync(string listId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));
        
        _logger.LogInformation("Getting list {ListId}", listId);
        
        var list = await _httpClient.GetAsync<List>($"list/{listId}", cancellationToken);
        
        _logger.LogInformation("Retrieved list: {ListName}", list.Name);
        return list;
    }
    
    public async Task<List> CreateListInFolderAsync(string folderId, CreateListRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folderId))
            throw new ArgumentException("Folder ID cannot be null or empty", nameof(folderId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        _logger.LogInformation("Creating list '{ListName}' in folder {FolderId}", request.Name, folderId);
        
        var list = await _httpClient.PostAsync<List>($"folder/{folderId}/list", request, cancellationToken);
        
        _logger.LogInformation("Created list: {ListName} (ID: {ListId})", list.Name, list.Id);
        return list;
    }
    
    public async Task<List> CreateListInSpaceAsync(string spaceId, CreateListRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(spaceId))
            throw new ArgumentException("Space ID cannot be null or empty", nameof(spaceId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        _logger.LogInformation("Creating list '{ListName}' in space {SpaceId}", request.Name, spaceId);
        
        var list = await _httpClient.PostAsync<List>($"space/{spaceId}/list", request, cancellationToken);
        
        _logger.LogInformation("Created list: {ListName} (ID: {ListId})", list.Name, list.Id);
        return list;
    }
    
    public async Task<List> UpdateListAsync(string listId, UpdateListRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        _logger.LogInformation("Updating list {ListId}", listId);
        
        var list = await _httpClient.PutAsync<List>($"list/{listId}", request, cancellationToken);
        
        _logger.LogInformation("Updated list: {ListName}", list.Name);
        return list;
    }
    
    public async Task DeleteListAsync(string listId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));
        
        _logger.LogInformation("Deleting list {ListId}", listId);
        
        await _httpClient.DeleteAsync($"list/{listId}", cancellationToken);
        
        _logger.LogInformation("Deleted list {ListId}", listId);
    }
}