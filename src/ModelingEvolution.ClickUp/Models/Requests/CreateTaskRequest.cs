using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record CreateTaskRequest
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    [JsonPropertyName("assignees")]
    public IReadOnlyList<int>? Assignees { get; init; }
    
    [JsonPropertyName("tags")]
    public IReadOnlyList<string>? Tags { get; init; }
    
    [JsonPropertyName("status")]
    public string? Status { get; init; }
    
    [JsonPropertyName("priority")]
    public int? Priority { get; init; }
    
    [JsonPropertyName("due_date")]
    public long? DueDate { get; init; }
    
    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; init; }
    
    [JsonPropertyName("time_estimate")]
    public long? TimeEstimate { get; init; }
    
    [JsonPropertyName("start_date")]
    public long? StartDate { get; init; }
    
    [JsonPropertyName("start_date_time")]
    public bool? StartDateTime { get; init; }
    
    [JsonPropertyName("notify_all")]
    public bool? NotifyAll { get; init; }
    
    [JsonPropertyName("parent")]
    public string? Parent { get; init; }
    
    [JsonPropertyName("links_to")]
    public string? LinksTo { get; init; }
    
    [JsonPropertyName("check_required_custom_fields")]
    public bool? CheckRequiredCustomFields { get; init; }
    
    [JsonPropertyName("custom_fields")]
    public IReadOnlyList<CustomFieldValue>? CustomFields { get; init; }
}

public record UpdateTaskRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    [JsonPropertyName("status")]
    public string? Status { get; init; }
    
    [JsonPropertyName("priority")]
    public int? Priority { get; init; }
    
    [JsonPropertyName("due_date")]
    public long? DueDate { get; init; }
    
    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; init; }
    
    [JsonPropertyName("time_estimate")]
    public long? TimeEstimate { get; init; }
    
    [JsonPropertyName("start_date")]
    public long? StartDate { get; init; }
    
    [JsonPropertyName("start_date_time")]
    public bool? StartDateTime { get; init; }
    
    [JsonPropertyName("assignees")]
    public UpdateTaskAssignees? Assignees { get; init; }
    
    [JsonPropertyName("archived")]
    public bool? Archived { get; init; }
}

public record UpdateTaskAssignees
{
    [JsonPropertyName("add")]
    public IReadOnlyList<int>? Add { get; init; }
    
    [JsonPropertyName("rem")]
    public IReadOnlyList<int>? Remove { get; init; }
}

public record CustomFieldValue
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("value")]
    public object? Value { get; init; }
}

public record SetCustomFieldRequest
{
    [JsonPropertyName("value")]
    public object? Value { get; init; }
}