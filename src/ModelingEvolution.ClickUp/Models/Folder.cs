using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record Folder
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("orderindex")]
    public int OrderIndex { get; init; }
    
    [JsonPropertyName("override_statuses")]
    public bool OverrideStatuses { get; init; }
    
    [JsonPropertyName("hidden")]
    public bool Hidden { get; init; }
    
    [JsonPropertyName("space")]
    public SpaceReference Space { get; init; } = new();
    
    [JsonPropertyName("task_count")]
    public string TaskCount { get; init; } = "0";
    
    [JsonPropertyName("archived")]
    public bool Archived { get; init; }
    
    [JsonPropertyName("lists")]
    public IReadOnlyList<List>? Lists { get; init; }
}

public record SpaceReference
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("access")]
    public bool Access { get; init; }
}