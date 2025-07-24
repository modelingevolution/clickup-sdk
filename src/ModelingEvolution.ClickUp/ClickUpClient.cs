using ModelingEvolution.ClickUp.Abstractions;
using ModelingEvolution.ClickUp.Clients;
using ModelingEvolution.ClickUp.Configuration;
using ModelingEvolution.ClickUp.Http;
using Microsoft.Extensions.Logging;

namespace ModelingEvolution.ClickUp;

public class ClickUpClient : IClickUpClient
{
    public IWorkspaceClient Workspaces { get; }
    public ISpaceClient Spaces { get; }
    public IFolderClient Folders { get; }
    public IListClient Lists { get; }
    public ITaskClient Tasks { get; }

    public ClickUpClient(
        HttpClient httpClient,
        ClickUpConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
        
        var clickUpHttpClient = new ClickUpHttpClient(
            httpClient,
            configuration,
            loggerFactory.CreateLogger<ClickUpHttpClient>());

        Workspaces = new WorkspaceClient(clickUpHttpClient, loggerFactory.CreateLogger<WorkspaceClient>());
        Spaces = new SpaceClient(clickUpHttpClient, loggerFactory.CreateLogger<SpaceClient>());
        Folders = new FolderClient(clickUpHttpClient, loggerFactory.CreateLogger<FolderClient>());
        Lists = new ListClient(clickUpHttpClient, loggerFactory.CreateLogger<ListClient>());
        Tasks = new TaskClient(clickUpHttpClient, loggerFactory.CreateLogger<TaskClient>());
    }
}