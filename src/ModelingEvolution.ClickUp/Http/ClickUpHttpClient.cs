using System.Net.Http.Headers;
using System.Text.Json;
using ModelingEvolution.ClickUp.Configuration;
using Microsoft.Extensions.Logging;

namespace ModelingEvolution.ClickUp.Http;

public class ClickUpHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ClickUpHttpClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ClickUpHttpClient(HttpClient httpClient, ClickUpConfiguration configuration, ILogger<ClickUpHttpClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _httpClient.BaseAddress = new Uri(configuration.BaseUrl.EndsWith('/') ? configuration.BaseUrl : configuration.BaseUrl + "/");
        _httpClient.Timeout = configuration.Timeout;
        _httpClient.DefaultRequestHeaders.Add("Authorization", configuration.ApiToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var fullUrl = new Uri(_httpClient.BaseAddress!, endpoint).ToString();
        _logger.LogInformation("GET {FullUrl} (BaseAddress: {BaseAddress}, Endpoint: {Endpoint})", fullUrl, _httpClient.BaseAddress, endpoint);
        
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Request failed with status {StatusCode}: {Content}", response.StatusCode, content);
        }
        
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogTrace("Response: {Content}", responseContent);
        
        var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        return result ?? throw new InvalidOperationException($"Failed to deserialize response to {typeof(T).Name}");
    }
}