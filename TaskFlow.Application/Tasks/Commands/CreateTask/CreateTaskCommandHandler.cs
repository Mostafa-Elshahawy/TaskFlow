using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.CreateTask;

internal class CreateTaskCommandHandler(
    ILogger<CreateTaskCommandHandler> logger,
    ITaskRepository taskRepository,
    IMapper mapper,
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

        var task = mapper.Map<TaskEntity>(request);
        task.CreatedByUserId = currentUser.Id;

        return await taskRepository.Create(task);
    }
}
