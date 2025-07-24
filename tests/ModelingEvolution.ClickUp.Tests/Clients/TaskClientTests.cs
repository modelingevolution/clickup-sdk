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

public class TaskClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly ClickUpConfiguration _configuration;
    private readonly Mock<ILogger<ClickUpHttpClient>> _httpClientLoggerMock;
    private readonly Mock<ILogger<TaskClient>> _taskClientLoggerMock;
    private readonly TaskClient _taskClient;

    public TaskClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _configuration = new ClickUpConfiguration
        {
            ApiToken = "test-token",
            BaseUrl = "https://api.clickup.com/api/v2"
        };
        _httpClientLoggerMock = new Mock<ILogger<ClickUpHttpClient>>();
        _taskClientLoggerMock = new Mock<ILogger<TaskClient>>();
        
        var clickUpHttpClient = new ClickUpHttpClient(_httpClient, _configuration, _httpClientLoggerMock.Object);
        _taskClient = new TaskClient(clickUpHttpClient, _taskClientLoggerMock.Object);
    }

    [Fact]
    public async Task GetTasksAsync_WithValidListId_ReturnsTasks()
    {
        // Arrange
        var listId = "list123";
        var tasks = new List<TaskItem>
        {
            new() { Id = "task1", Name = "Task 1", Description = "Description 1" },
            new() { Id = "task2", Name = "Task 2", Description = "Description 2" }
        };
        var response = new TasksResponse { Tasks = tasks, LastPage = true };
        var jsonResponse = JsonSerializer.Serialize(response);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith($"/list/{listId}/task?page=0")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _taskClient.GetTasksAsync(listId);

        // Assert
        result.Should().NotBeNull();
        result.Tasks.Should().HaveCount(2);
        result.LastPage.Should().BeTrue();
        result.Tasks[0].Id.Should().Be("task1");
        result.Tasks[0].Name.Should().Be("Task 1");
        result.Tasks[1].Id.Should().Be("task2");
        result.Tasks[1].Name.Should().Be("Task 2");
    }

    [Fact]
    public async Task GetTasksAsync_WithEmptyListId_ThrowsArgumentException()
    {
        // Act & Assert
        await _taskClient.Invoking(c => c.GetTasksAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("List ID cannot be null or empty*");
    }

    [Fact]
    public async Task GetTaskAsync_WithValidTaskId_ReturnsTask()
    {
        // Arrange
        var taskId = "task123";
        var task = new TaskItem 
        { 
            Id = taskId, 
            Name = "Test Task", 
            Description = "Test Description",
            Status = new Status { StatusName = "Open" }
        };
        var jsonResponse = JsonSerializer.Serialize(task);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith($"/task/{taskId}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _taskClient.GetTaskAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(taskId);
        result.Name.Should().Be("Test Task");
        result.Description.Should().Be("Test Description");
        result.Status!.StatusName.Should().Be("Open");
    }

    [Fact]
    public async Task GetTaskAsync_WithEmptyTaskId_ThrowsArgumentException()
    {
        // Act & Assert
        await _taskClient.Invoking(c => c.GetTaskAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Task ID cannot be null or empty*");
    }

    [Fact]
    public async Task GetTasksAsync_WithPagination_IncludesPageParameter()
    {
        // Arrange
        var listId = "list123";
        var page = 2;
        var response = new TasksResponse { Tasks = new List<TaskItem>(), LastPage = false };
        var jsonResponse = JsonSerializer.Serialize(response);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith($"/list/{listId}/task?page={page}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _taskClient.GetTasksAsync(listId, page);

        // Assert
        result.Should().NotBeNull();
        result.LastPage.Should().BeFalse();
    }
}