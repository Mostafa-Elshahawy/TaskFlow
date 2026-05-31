using MediatR;
using TaskFlow.Application.Projects.Dtos;

namespace TaskFlow.Application.Projects.Queries.GetProjects;

public class GetProjectsQuery : IRequest<IEnumerable<ProjectDto>>
{
    public int? OrganizationId { get; set; }
    public string? CreatedByUserId { get; set; }
}
