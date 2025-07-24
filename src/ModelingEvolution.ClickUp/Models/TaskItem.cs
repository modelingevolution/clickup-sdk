using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record TaskItem
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("text_content")]
    public string? TextContent { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    [JsonPropertyName("status")]
    public Status? Status { get; init; }
    
    [JsonPropertyName("orderindex")]
    public string? OrderIndex { get; init; }
    
    [JsonPropertyName("date_created")]
    public string? DateCreated { get; init; }
    
    [JsonPropertyName("date_updated")]
    public string? DateUpdated { get; init; }
    
    [JsonPropertyName("date_closed")]
    public string? DateClosed { get; init; }
    
    [JsonPropertyName("archived")]
    public bool Archived { get; init; }
    
    [JsonPropertyName("creator")]
    public User? Creator { get; init; }
    
    [JsonPropertyName("assignees")]
    public IReadOnlyList<User>? Assignees { get; init; }
    
    [JsonPropertyName("watchers")]
    public IReadOnlyList<User>? Watchers { get; init; }
    
    [JsonPropertyName("checklists")]
    public IReadOnlyList<object>? Checklists { get; init; }
    
    [JsonPropertyName("tags")]
    public IReadOnlyList<Tag>? Tags { get; init; }
    
    [JsonPropertyName("parent")]
    public string? Parent { get; init; }
    
    [JsonPropertyName("priority")]
    public Priority? Priority { get; init; }
    
    [JsonPropertyName("due_date")]
    public string? DueDate { get; init; }
    
    [JsonPropertyName("start_date")]
    public string? StartDate { get; init; }
    
    [JsonPropertyName("points")]
    public decimal? Points { get; init; }
    
    [JsonPropertyName("time_estimate")]
    public long? TimeEstimate { get; init; }
    
    [JsonPropertyName("time_spent")]
    public long? TimeSpent { get; init; }
    
    [JsonPropertyName("custom_fields")]
    public IReadOnlyList<CustomField>? CustomFields { get; init; }
    
    [JsonPropertyName("dependencies")]
    public IReadOnlyList<object>? Dependencies { get; init; }
    
    [JsonPropertyName("linked_tasks")]
    public IReadOnlyList<object>? LinkedTasks { get; init; }
    
    [JsonPropertyName("team_id")]
    public string? TeamId { get; init; }
    
    [JsonPropertyName("url")]
    public string? Url { get; init; }
    
    [JsonPropertyName("permission_level")]
    public string? PermissionLevel { get; init; }
    
    [JsonPropertyName("list")]
    public ListReference? List { get; init; }
    
    [JsonPropertyName("project")]
    public ProjectReference? Project { get; init; }
    
    [JsonPropertyName("folder")]
    public FolderReference? Folder { get; init; }
    
    [JsonPropertyName("space")]
    public SpaceReference? Space { get; init; }
}

public record TasksResponse
{
    [JsonPropertyName("tasks")]
    public IReadOnlyList<TaskItem> Tasks { get; init; } = Array.Empty<TaskItem>();
    
    [JsonPropertyName("last_page")]
    public bool LastPage { get; init; }
}

public record User
{
    [JsonPropertyName("id")]
    public long Id { get; init; }
    
    [JsonPropertyName("username")]
    public string Username { get; init; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string? Email { get; init; }
    
    [JsonPropertyName("color")]
    public string? Color { get; init; }
    
    [JsonPropertyName("profilePicture")]
    public string? ProfilePicture { get; init; }
    
    [JsonPropertyName("initials")]
    public string? Initials { get; init; }
}

public record Tag
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("tag_fg")]
    public string? TagFg { get; init; }
    
    [JsonPropertyName("tag_bg")]
    public string? TagBg { get; init; }
    
    [JsonPropertyName("creator")]
    public long? Creator { get; init; }
}

public record CustomField
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;
    
    [JsonPropertyName("type_config")]
    public object? TypeConfig { get; init; }
    
    [JsonPropertyName("date_created")]
    public string? DateCreated { get; init; }
    
    [JsonPropertyName("hide_from_guests")]
    public bool HideFromGuests { get; init; }
    
    [JsonPropertyName("value")]
    public object? Value { get; init; }
    
    [JsonPropertyName("required")]
    public bool Required { get; init; }
}

public record ListReference
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("access")]
    public bool Access { get; init; }
}

public record ProjectReference
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