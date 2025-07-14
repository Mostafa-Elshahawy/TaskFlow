using MediatR;
using TaskFlow.Application.Projects.Dtos;

namespace TaskFlow.Application.Projects.Queries.GetProjects;

public class GetProjectsQuery : IRequest<IEnumerable<ProjectDto>>
{
    public int Id { set; get; }
    public string Name { set; get; } = default!;
    public string Description { set; get; } = default!;
    public string CreatedById { set; get; } = default!;
    public string UpdatedById { set; get; } = default!;
    public DateTime CreatedAt { set; get; }
    public DateTime? UpdatedAt { set; get; }
    public bool IsDeleted { set; get; } = false;
    public int OrganizationId { set; get; }
}