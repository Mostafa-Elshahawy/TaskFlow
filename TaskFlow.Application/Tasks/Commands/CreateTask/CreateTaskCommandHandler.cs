using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Tasks.Dtos;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.CreateTask;

internal class CreateTaskCommandHandler(
    ILogger<CreateTaskCommandHandler> logger,
    ITaskRepository taskRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<CreateTaskCommand, int>
{
    public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var project = string.IsNullOrWhiteSpace(request.AssigneeId)
            ? await accessService.RequireProjectContributionAsync(request.ProjectId, cancellationToken)
            : await accessService.RequireProjectManagementAsync(request.ProjectId, cancellationToken);
        var currentUser = accessService.GetCurrentUser();

        if (!string.IsNullOrWhiteSpace(request.AssigneeId))
            await accessService.EnsureProjectMemberAsync(project, request.AssigneeId);

        logger.LogInformation(
            "User {UserId} is creating a task in project {ProjectId}",
            currentUser.Id,
            project.Id);

        var task = request.ToEntity(currentUser.Id, DateTime.UtcNow);

        return await taskRepository.Create(task);
    }
}
