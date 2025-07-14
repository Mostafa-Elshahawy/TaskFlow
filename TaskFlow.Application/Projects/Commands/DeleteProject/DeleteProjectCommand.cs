using MediatR;

namespace TaskFlow.Application.Projects.Commands.DeleteProject;

public class DeleteProjectCommand(int id) : IRequest
{
    public int Id { get; } = id;
}