using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record Space
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("private")]
    public bool Private { get; init; }
    
    [JsonPropertyName("color")]
    public string? Color { get; init; }
    
    [JsonPropertyName("avatar")]
    public string? Avatar { get; init; }
    
    [JsonPropertyName("admin_can_manage")]
    public bool? AdminCanManage { get; init; }
    
    [JsonPropertyName("archived")]
    public bool Archived { get; init; }
}