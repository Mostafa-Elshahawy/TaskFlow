using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Projects.Dtos;

namespace TaskFlow.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler(
    ILogger<GetProjectByIdQueryHandler> logger,
    IWorkspaceAccessService accessService) : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching project with ID: {ProjectId}", request.Id);

        var project = await accessService.RequireProjectViewAsync(request.Id, cancellationToken);
        var projectDto = project.ToDto();

        logger.LogInformation("Project fetched successfully: {ProjectId}", request.Id);
        return projectDto;
    }
}
