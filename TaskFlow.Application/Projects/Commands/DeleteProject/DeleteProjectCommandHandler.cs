using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Common;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler(
    ILogger<DeleteProjectCommandHandler> logger,
    IProjectRepository projectRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var expectedRowVersion = ConcurrencyToken.Decode(request.RowVersion);
        var project = await projectRepository.GetProjectByIdForMutationAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskFlow.Domain.Entites.Project), request.Id.ToString());
        await accessService.RequireProjectManagementAsync(project, cancellationToken);
        if (project.isDeleted)
            throw new ConflictException("The project is no longer available. Reload and try again.");

        logger.LogInformation("Deleting project with ID {ProjectId}", request.Id);
        await projectRepository.DeleteProjectAsync(project.Id, expectedRowVersion, cancellationToken);
    }
}
