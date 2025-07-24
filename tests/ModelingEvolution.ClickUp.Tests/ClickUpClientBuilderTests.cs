using FluentAssertions;
using Microsoft.Extensions.Logging;
using ModelingEvolution.ClickUp;

namespace ModelingEvolution.ClickUp.Tests;

public class ClickUpClientBuilderTests
{
    [Fact]
    public void Build_WithoutApiToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new ClickUpClientBuilder();

        // Act & Assert
        builder.Invoking(b => b.Build())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("API token is required");
    }

    [Fact]
    public void Build_WithApiToken_CreatesClient()
    {
        // Arrange
        var builder = new ClickUpClientBuilder()
            .WithApiToken("test-token");

        // Act
        var client = builder.Build();

        // Assert
        client.Should().NotBeNull();
        client.Workspaces.Should().NotBeNull();
        client.Spaces.Should().NotBeNull();
        client.Folders.Should().NotBeNull();
        client.Lists.Should().NotBeNull();
        client.Tasks.Should().NotBeNull();
    }

    [Fact]
    public void Build_WithCustomConfiguration_CreatesClientWithConfiguration()
    {
        // Arrange
        var loggerFactory = LoggerFactory.Create(builder => { });
        var httpClient = new HttpClient();
        var baseUrl = "https://custom.api.clickup.com";
        var timeout = TimeSpan.FromMinutes(2);

        var builder = new ClickUpClientBuilder()
            .WithApiToken("test-token")
            .WithBaseUrl(baseUrl)
            .WithTimeout(timeout)
            .WithLoggerFactory(loggerFactory)
            .WithHttpClient(httpClient);

        // Act
        var client = builder.Build();

        // Assert
        client.Should().NotBeNull();
    }
}