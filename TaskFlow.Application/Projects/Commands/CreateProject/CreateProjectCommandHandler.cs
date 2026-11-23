using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler(
    ILogger<CreateProjectCommandHandler> logger,
    IMapper mapper,
    IProjectRepository projectRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<CreateProjectCommand, int>
{
    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        await accessService.RequireProjectCreationAsync(request.OrganizationId, cancellationToken);
        var currentUser = accessService.GetCurrentUser();

        logger.LogInformation(
            "User {UserId} is creating project {ProjectName} in organization {OrganizationId}",
            currentUser.Id,
            request.Name,
            request.OrganizationId);

        var project = mapper.Map<Project>(request);
        project.CreatedByUserId = currentUser.Id;
        project.Members = new List<ProjectMember>
        {
            new()
            {
                UserId = currentUser.Id,
                AddedAt = DateTime.UtcNow
            }
        };

        var id = await projectRepository.CreateProjectAsync(project);
        logger.LogInformation("Project created with ID {ProjectId}", id);
        return id;
    }
}
