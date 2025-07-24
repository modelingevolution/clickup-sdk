namespace ModelingEvolution.ClickUp.Abstractions;

public interface IClickUpClient
{
    IWorkspaceClient Workspaces { get; }
    ISpaceClient Spaces { get; }
    IFolderClient Folders { get; }
    IListClient Lists { get; }
    ITaskClient Tasks { get; }
}