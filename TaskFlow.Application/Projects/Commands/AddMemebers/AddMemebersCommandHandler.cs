using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Commands.AddMemebers;

public class AddMemebersCommandHandler(
    ILogger<AddMemebersCommandHandler> logger,
    IProjectRepository projectRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<AddMemebersCommand>
{
    public async Task Handle(AddMemebersCommand request, CancellationToken cancellationToken)
    {
        var project = await accessService.RequireProjectManagementAsync(request.ProjectId, cancellationToken);
        await accessService.EnsureOrganizationMemberAsync(project.OrganizationId, request.UserId, cancellationToken);

        logger.LogInformation(
            "Adding user {UserId} to project {ProjectId}",
            request.UserId,
            request.ProjectId);

        await projectRepository.AddMemberAsync(project.Id, request.UserId, cancellationToken);
    }
}
