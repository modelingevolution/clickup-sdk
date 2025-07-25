using ModelingEvolution.ClickUp.Abstractions;
using ModelingEvolution.ClickUp.Http;
using ModelingEvolution.ClickUp.Models;
using Microsoft.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Clients;

public class FolderClient : IFolderClient
{
    private readonly ClickUpHttpClient _httpClient;
    private readonly ILogger<FolderClient> _logger;

    public FolderClient(ClickUpHttpClient httpClient, ILogger<FolderClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<Folder>> GetFoldersAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(spaceId))
            throw new ArgumentException("Space ID cannot be null or empty", nameof(spaceId));
        
        _logger.LogInformation("Getting folders for space {SpaceId}", spaceId);
        
        var response = await _httpClient.GetAsync<FoldersResponse>($"space/{spaceId}/folder", cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} folders", response.Folders.Count);
        return response.Folders;
    }

    public async Task<Folder> GetFolderAsync(string folderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folderId))
            throw new ArgumentException("Folder ID cannot be null or empty", nameof(folderId));
        
        _logger.LogInformation("Getting folder {FolderId}", folderId);
        
        var folder = await _httpClient.GetAsync<Folder>($"folder/{folderId}", cancellationToken);
        
        _logger.LogInformation("Retrieved folder: {FolderName}", folder.Name);
        return folder;
    }
    
    public async Task<Folder> CreateFolderAsync(string spaceId, CreateFolderRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(spaceId))
            throw new ArgumentException("Space ID cannot be null or empty", nameof(spaceId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        _logger.LogInformation("Creating folder '{FolderName}' in space {SpaceId}", request.Name, spaceId);
        
        var folder = await _httpClient.PostAsync<Folder>($"space/{spaceId}/folder", request, cancellationToken);
        
        _logger.LogInformation("Created folder: {FolderName} (ID: {FolderId})", folder.Name, folder.Id);
        return folder;
    }
    
    public async Task<Folder> UpdateFolderAsync(string folderId, UpdateFolderRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folderId))
            throw new ArgumentException("Folder ID cannot be null or empty", nameof(folderId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        _logger.LogInformation("Updating folder {FolderId}", folderId);
        
        var folder = await _httpClient.PutAsync<Folder>($"folder/{folderId}", request, cancellationToken);
        
        _logger.LogInformation("Updated folder: {FolderName}", folder.Name);
        return folder;
    }
    
    public async Task DeleteFolderAsync(string folderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folderId))
            throw new ArgumentException("Folder ID cannot be null or empty", nameof(folderId));
        
        _logger.LogInformation("Deleting folder {FolderId}", folderId);
        
        await _httpClient.DeleteAsync($"folder/{folderId}", cancellationToken);
        
        _logger.LogInformation("Deleted folder {FolderId}", folderId);
    }
}