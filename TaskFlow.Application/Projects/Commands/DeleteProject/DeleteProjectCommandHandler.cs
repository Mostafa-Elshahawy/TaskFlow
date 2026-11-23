using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler(
    ILogger<DeleteProjectCommandHandler> logger,
    IProjectRepository projectRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await accessService.RequireProjectManagementAsync(request.Id, cancellationToken);

        logger.LogInformation("Deleting project with ID {ProjectId}", request.Id);
        await projectRepository.DeleteProjectAsync(project.Id, cancellationToken);
    }
}
