namespace ModelingEvolution.ClickUp.Configuration;

public class ClickUpConfiguration
{
    public string ApiToken { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = "https://api.clickup.com/api/v2/";
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
}