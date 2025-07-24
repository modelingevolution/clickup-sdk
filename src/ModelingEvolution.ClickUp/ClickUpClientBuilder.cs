using ModelingEvolution.ClickUp.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ModelingEvolution.ClickUp;

public class ClickUpClientBuilder
{
    private string _apiToken = string.Empty;
    private string _baseUrl = "https://api.clickup.com/api/v2";
    private TimeSpan _timeout = TimeSpan.FromSeconds(30);
    private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
    private HttpClient? _httpClient;

    public ClickUpClientBuilder WithApiToken(string apiToken)
    {
        _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
        return this;
    }

    public ClickUpClientBuilder WithBaseUrl(string baseUrl)
    {
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        return this;
    }

    public ClickUpClientBuilder WithTimeout(TimeSpan timeout)
    {
        _timeout = timeout;
        return this;
    }

    public ClickUpClientBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        return this;
    }

    public ClickUpClientBuilder WithHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        return this;
    }

    public ClickUpClient Build()
    {
        if (string.IsNullOrWhiteSpace(_apiToken))
            throw new InvalidOperationException("API token is required");

        var configuration = new ClickUpConfiguration
        {
            ApiToken = _apiToken,
            BaseUrl = _baseUrl,
            Timeout = _timeout
        };

        var httpClient = _httpClient ?? new HttpClient();
        
        return new ClickUpClient(httpClient, configuration, _loggerFactory);
    }
}