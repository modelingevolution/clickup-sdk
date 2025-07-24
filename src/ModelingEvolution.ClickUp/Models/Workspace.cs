using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record Workspace
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("color")]
    public string? Color { get; init; }
    
    [JsonPropertyName("avatar")]
    public string? Avatar { get; init; }
}