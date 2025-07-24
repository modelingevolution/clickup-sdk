using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record List
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("orderindex")]
    public int OrderIndex { get; init; }
    
    [JsonPropertyName("content")]
    public string? Content { get; init; }
    
    [JsonPropertyName("status")]
    public Status? Status { get; init; }
    
    [JsonPropertyName("priority")]
    public Priority? Priority { get; init; }
    
    [JsonPropertyName("assignee")]
    public object? Assignee { get; init; }
    
    [JsonPropertyName("task_count")]
    public int? TaskCount { get; init; }
    
    [JsonPropertyName("due_date")]
    public string? DueDate { get; init; }
    
    [JsonPropertyName("start_date")]
    public string? StartDate { get; init; }
    
    [JsonPropertyName("folder")]
    public FolderReference? Folder { get; init; }
    
    [JsonPropertyName("space")]
    public SpaceReference Space { get; init; } = new();
    
    [JsonPropertyName("archived")]
    public bool Archived { get; init; }
    
    [JsonPropertyName("override_statuses")]
    public bool OverrideStatuses { get; init; }
    
    [JsonPropertyName("permission_level")]
    public string? PermissionLevel { get; init; }
}

public record FolderReference
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("hidden")]
    public bool Hidden { get; init; }
    
    [JsonPropertyName("access")]
    public bool Access { get; init; }
}

public record Status
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }
    
    [JsonPropertyName("status")]
    public string StatusName { get; init; } = string.Empty;
    
    [JsonPropertyName("color")]
    public string? Color { get; init; }
    
    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; init; }
    
    [JsonPropertyName("type")]
    public string? Type { get; init; }
}

public record Priority
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }
    
    [JsonPropertyName("priority")]
    public string PriorityName { get; init; } = string.Empty;
    
    [JsonPropertyName("color")]
    public string? Color { get; init; }
    
    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; init; }
}