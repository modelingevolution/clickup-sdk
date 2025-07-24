using FluentAssertions;
using Microsoft.Extensions.Logging;
using ModelingEvolution.ClickUp.Clients;
using ModelingEvolution.ClickUp.Configuration;
using ModelingEvolution.ClickUp.Http;
using ModelingEvolution.ClickUp.Models;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace ModelingEvolution.ClickUp.Tests.Clients;

public class WorkspaceClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly ClickUpConfiguration _configuration;
    private readonly Mock<ILogger<ClickUpHttpClient>> _httpClientLoggerMock;
    private readonly Mock<ILogger<WorkspaceClient>> _workspaceClientLoggerMock;
    private readonly WorkspaceClient _workspaceClient;

    public WorkspaceClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _configuration = new ClickUpConfiguration
        {
            ApiToken = "test-token",
            BaseUrl = "https://api.clickup.com/api/v2"
        };
        _httpClientLoggerMock = new Mock<ILogger<ClickUpHttpClient>>();
        _workspaceClientLoggerMock = new Mock<ILogger<WorkspaceClient>>();
        
        var clickUpHttpClient = new ClickUpHttpClient(_httpClient, _configuration, _httpClientLoggerMock.Object);
        _workspaceClient = new WorkspaceClient(clickUpHttpClient, _workspaceClientLoggerMock.Object);
    }

    [Fact]
    public async Task GetWorkspacesAsync_ReturnsWorkspaces()
    {
        // Arrange
        var workspaces = new List<Workspace>
        {
            new() { Id = "123", Name = "Test Workspace 1" },
            new() { Id = "456", Name = "Test Workspace 2" }
        };
        var response = new WorkspacesResponse { Teams = workspaces };
        var jsonResponse = JsonSerializer.Serialize(response);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith("/team")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _workspaceClient.GetWorkspacesAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be("123");
        result[0].Name.Should().Be("Test Workspace 1");
        result[1].Id.Should().Be("456");
        result[1].Name.Should().Be("Test Workspace 2");
    }

    [Fact]
    public async Task GetWorkspacesAsync_WithHttpError_ThrowsHttpRequestException()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("Unauthorized")
            });

        // Act & Assert
        await _workspaceClient.Invoking(c => c.GetWorkspacesAsync())
            .Should().ThrowAsync<HttpRequestException>();
    }
}