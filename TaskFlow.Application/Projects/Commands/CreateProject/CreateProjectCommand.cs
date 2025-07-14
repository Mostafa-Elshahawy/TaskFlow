using MediatR;

namespace TaskFlow.Application.Projects.Commands.CreateProject;

public class CreateProjectCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } 
    public int OrganizationId { get; set; } 
    public bool IsDeleted { get; set; } = false;
}