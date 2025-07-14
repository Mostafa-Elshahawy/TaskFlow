using MediatR;

namespace TaskFlow.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}