using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Common;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler(
    ILogger<UpdateProjectCommandHandler> logger,
    IProjectRepository projectRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<UpdateProjectCommand>
{
    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var expectedRowVersion = ConcurrencyToken.Decode(request.RowVersion);
        var project = await projectRepository.GetProjectByIdForMutationAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskFlow.Domain.Entites.Project), request.Id.ToString());
        await accessService.RequireProjectManagementAsync(project, cancellationToken);
        if (project.isDeleted)
            throw new ConflictException("The project is no longer available. Reload and try again.");

        await projectRepository.UpdateDetailsAsync(
            project.Id,
            request.Name,
            request.Description,
            expectedRowVersion,
            cancellationToken);
        logger.LogInformation("Updated project with ID {ProjectId}", request.Id);
    }
}
