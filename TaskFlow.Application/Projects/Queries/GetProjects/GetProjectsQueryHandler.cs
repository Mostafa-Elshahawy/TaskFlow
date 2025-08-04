using AutoMapper;
using MediatR;
using TaskFlow.Application.Projects.Dtos;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler(IMapper mapper,IProjectRepository projectRepository) : IRequestHandler<GetProjectsQuery, IEnumerable<ProjectDto>>
{
    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await projectRepository.GetAllProjectsAsync();
        return mapper.Map<IEnumerable<ProjectDto>>(projects);
    }
}
