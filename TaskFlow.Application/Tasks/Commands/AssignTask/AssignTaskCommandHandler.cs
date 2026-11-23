using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.AssignTask;

public class AssignTaskCommandHandler(
    ILogger<AssignTaskCommandHandler> logger,
    ITaskRepository taskRepository,
    UserManager<ApplicationUser> userManager,
    IWorkspaceAccessService accessService) : IRequestHandler<AssignTaskCommand>
{
    public async Task Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var currentUser = accessService.GetCurrentUser();
        var task = await taskRepository.GetById(request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task", request.TaskId.ToString());

        var project = await accessService.RequireProjectManagementAsync(task.ProjectId, cancellationToken);
        await accessService.EnsureProjectMemberAsync(project, request.AssigneeId);

        var assignee = await userManager.FindByIdAsync(request.AssigneeId)
            ?? throw new NotFoundException(nameof(ApplicationUser), request.AssigneeId);

        task.AssigneeId = assignee.Id;
        task.UpdatedAt = DateTime.UtcNow;

        await taskRepository.AssignTask(task);
        logger.LogInformation(
            "User {UserId} assigned task {TaskId} to {AssigneeId}",
            currentUser.Id,
            request.TaskId,
            assignee.Id);
    }
}
