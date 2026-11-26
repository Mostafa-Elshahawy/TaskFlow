using MediatR;

namespace TaskFlow.Application.Projects.Commands.DeleteProject;

public class DeleteProjectCommand(int id, string rowVersion) : IRequest
{
    public int Id { get; } = id;
    public string RowVersion { get; } = rowVersion;
}
