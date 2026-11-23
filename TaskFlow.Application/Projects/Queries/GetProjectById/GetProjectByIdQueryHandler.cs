using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Projects.Dtos;

namespace TaskFlow.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler(
    ILogger<GetProjectByIdQueryHandler> logger,
    IMapper mapper,
    IWorkspaceAccessService accessService) : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching project with ID: {ProjectId}", request.Id);

        var project = await accessService.RequireProjectViewAsync(request.Id, cancellationToken);
        var projectDto = mapper.Map<ProjectDto>(project);

        logger.LogInformation("Project fetched successfully: {ProjectId}", request.Id);
        return projectDto;
    }
}
