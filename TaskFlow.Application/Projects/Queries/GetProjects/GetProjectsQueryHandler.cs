using AutoMapper;
using MediatR;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Projects.Dtos;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler(
    IMapper mapper,
    IProjectRepository projectRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<GetProjectsQuery, IEnumerable<ProjectDto>>
{
    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = accessService.GetCurrentUser();
        var projects = await projectRepository.GetAllProjectsForUserAsync(
            currentUser.Id,
            request.OrganizationId,
            request.CreatedByUserId,
            cancellationToken);

        return mapper.Map<IEnumerable<ProjectDto>>(projects);
    }
}
