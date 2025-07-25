using System.Text.Json.Serialization;

namespace ModelingEvolution.ClickUp.Models;

public record CreateSpaceRequest
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("multiple_assignees")]
    public bool? MultipleAssignees { get; init; }
    
    [JsonPropertyName("features")]
    public SpaceFeatures? Features { get; init; }
}

public record UpdateSpaceRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    
    [JsonPropertyName("color")]
    public string? Color { get; init; }
    
    [JsonPropertyName("private")]
    public bool? Private { get; init; }
    
    [JsonPropertyName("admin_can_manage")]
    public bool? AdminCanManage { get; init; }
    
    [JsonPropertyName("multiple_assignees")]
    public bool? MultipleAssignees { get; init; }
    
    [JsonPropertyName("features")]
    public SpaceFeatures? Features { get; init; }
}

public record SpaceFeatures
{
    [JsonPropertyName("due_dates")]
    public FeatureSetting? DueDates { get; init; }
    
    [JsonPropertyName("time_tracking")]
    public FeatureSetting? TimeTracking { get; init; }
    
    [JsonPropertyName("tags")]
    public FeatureSetting? Tags { get; init; }
    
    [JsonPropertyName("time_estimates")]
    public FeatureSetting? TimeEstimates { get; init; }
    
    [JsonPropertyName("checklists")]
    public FeatureSetting? Checklists { get; init; }
    
    [JsonPropertyName("custom_fields")]
    public FeatureSetting? CustomFields { get; init; }
    
    [JsonPropertyName("remap_dependencies")]
    public FeatureSetting? RemapDependencies { get; init; }
    
    [JsonPropertyName("dependency_warning")]
    public FeatureSetting? DependencyWarning { get; init; }
    
    [JsonPropertyName("portfolios")]
    public FeatureSetting? Portfolios { get; init; }
}

public record FeatureSetting
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }
}