using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record CreateFolderRequest
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

public record UpdateFolderRequest  
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}