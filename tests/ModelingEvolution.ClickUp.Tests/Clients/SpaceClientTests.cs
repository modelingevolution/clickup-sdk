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

public class SpaceClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly ClickUpConfiguration _configuration;
    private readonly Mock<ILogger<ClickUpHttpClient>> _httpClientLoggerMock;
    private readonly Mock<ILogger<SpaceClient>> _spaceClientLoggerMock;
    private readonly SpaceClient _spaceClient;

    public SpaceClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _configuration = new ClickUpConfiguration
        {
            ApiToken = "test-token",
            BaseUrl = "https://api.clickup.com/api/v2"
        };
        _httpClientLoggerMock = new Mock<ILogger<ClickUpHttpClient>>();
        _spaceClientLoggerMock = new Mock<ILogger<SpaceClient>>();
        
        var clickUpHttpClient = new ClickUpHttpClient(_httpClient, _configuration, _httpClientLoggerMock.Object);
        _spaceClient = new SpaceClient(clickUpHttpClient, _spaceClientLoggerMock.Object);
    }

    [Fact]
    public async Task GetSpacesAsync_WithValidWorkspaceId_ReturnsSpaces()
    {
        // Arrange
        var workspaceId = "workspace123";
        var spaces = new List<Space>
        {
            new() { Id = "space1", Name = "Development", Private = false },
            new() { Id = "space2", Name = "Design", Private = true }
        };
        var response = new SpacesResponse { Spaces = spaces };
        var jsonResponse = JsonSerializer.Serialize(response);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith($"/team/{workspaceId}/space")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _spaceClient.GetSpacesAsync(workspaceId);

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be("space1");
        result[0].Name.Should().Be("Development");
        result[0].Private.Should().BeFalse();
        result[1].Id.Should().Be("space2");
        result[1].Name.Should().Be("Design");
        result[1].Private.Should().BeTrue();
    }

    [Fact]
    public async Task GetSpacesAsync_WithEmptyWorkspaceId_ThrowsArgumentException()
    {
        // Act & Assert
        await _spaceClient.Invoking(c => c.GetSpacesAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Workspace ID cannot be null or empty*");
    }

    [Fact]
    public async Task GetSpaceAsync_WithValidSpaceId_ReturnsSpace()
    {
        // Arrange
        var spaceId = "space123";
        var space = new Space { Id = spaceId, Name = "Test Space", Private = false };
        var jsonResponse = JsonSerializer.Serialize(space);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith($"/space/{spaceId}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _spaceClient.GetSpaceAsync(spaceId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(spaceId);
        result.Name.Should().Be("Test Space");
        result.Private.Should().BeFalse();
    }

    [Fact]
    public async Task GetSpaceAsync_WithEmptySpaceId_ThrowsArgumentException()
    {
        // Act & Assert
        await _spaceClient.Invoking(c => c.GetSpaceAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Space ID cannot be null or empty*");
    }
}