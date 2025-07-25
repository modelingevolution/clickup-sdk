using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record CreateListRequest
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("content")]
    public string? Content { get; init; }
    
    [JsonPropertyName("due_date")]
    public long? DueDate { get; init; }
    
    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; init; }
    
    [JsonPropertyName("priority")]
    public int? Priority { get; init; }
    
    [JsonPropertyName("assignee")]
    public int? Assignee { get; init; }
    
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}

public record UpdateListRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    
    [JsonPropertyName("content")]
    public string? Content { get; init; }
    
    [JsonPropertyName("due_date")]
    public long? DueDate { get; init; }
    
    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; init; }
    
    [JsonPropertyName("priority")]
    public int? Priority { get; init; }
    
    [JsonPropertyName("assignee")]
    public UpdateAssignee? Assignee { get; init; }
    
    [JsonPropertyName("unset_status")]
    public bool? UnsetStatus { get; init; }
}

public record UpdateAssignee
{
    [JsonPropertyName("add")]
    public int? Add { get; init; }
    
    [JsonPropertyName("rem")]
    public int? Remove { get; init; }
}