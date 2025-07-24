using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record WorkspacesResponse
{
    [JsonPropertyName("teams")]
    public IReadOnlyList<Workspace> Teams { get; init; } = Array.Empty<Workspace>();
}

public record SpacesResponse
{
    [JsonPropertyName("spaces")]
    public IReadOnlyList<Space> Spaces { get; init; } = Array.Empty<Space>();
}

public record FoldersResponse
{
    [JsonPropertyName("folders")]
    public IReadOnlyList<Folder> Folders { get; init; } = Array.Empty<Folder>();
}

public record ListsResponse
{
    [JsonPropertyName("lists")]
    public IReadOnlyList<List> Lists { get; init; } = Array.Empty<List>();
}