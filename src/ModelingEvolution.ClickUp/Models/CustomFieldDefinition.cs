using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record CustomFieldDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;
    
    [JsonPropertyName("type_config")]
    public CustomFieldTypeConfig? TypeConfig { get; init; }
    
    [JsonPropertyName("date_created")]
    public string? DateCreated { get; init; }
    
    [JsonPropertyName("hide_from_guests")]
    public bool HideFromGuests { get; init; }
    
    [JsonPropertyName("required")]
    public bool Required { get; init; }
}

public record CustomFieldTypeConfig
{
    [JsonPropertyName("default")]
    public int? Default { get; init; }
    
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; init; }
    
    [JsonPropertyName("options")]
    public IReadOnlyList<CustomFieldOption>? Options { get; init; }
}

public record CustomFieldOption
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string? Value { get; init; }
    
    [JsonPropertyName("color")]
    public string? Color { get; init; }
    
    [JsonPropertyName("orderindex")]
    public int OrderIndex { get; init; }
}

public record CustomFieldsResponse
{
    [JsonPropertyName("fields")]
    public IReadOnlyList<CustomFieldDefinition> Fields { get; init; } = Array.Empty<CustomFieldDefinition>();
}