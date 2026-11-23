using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler(
    ILogger<UpdateProjectCommandHandler> logger,
    IProjectRepository projectRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<UpdateProjectCommand>
{
    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await accessService.RequireProjectManagementAsync(request.Id, cancellationToken);

        await projectRepository.UpdateDetailsAsync(
            project.Id,
            request.Name,
            request.Description,
            cancellationToken);
        logger.LogInformation("Updated project with ID {ProjectId}", request.Id);
    }
}
