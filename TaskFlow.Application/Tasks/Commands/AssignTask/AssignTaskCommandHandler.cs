using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Common;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.AssignTask;

public class AssignTaskCommandHandler(
    ILogger<AssignTaskCommandHandler> logger,
    ITaskRepository taskRepository,
    UserManager<ApplicationUser> userManager,
    IWorkspaceAccessService accessService,
    TimeProvider timeProvider) : IRequestHandler<AssignTaskCommand>
{
    public async Task Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var expectedRowVersion = ConcurrencyToken.Decode(request.RowVersion);
        var currentUser = accessService.GetCurrentUser();
        var task = await taskRepository.GetByIdForMutation(request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task", request.TaskId.ToString());

        var project = await accessService.RequireProjectManagementAsync(task.Project, cancellationToken);
        if (task.isDeleted || project.isDeleted)
            throw new ConflictException("The task is no longer available. Reload and try again.");

        await accessService.EnsureProjectMemberAsync(project, request.AssigneeId);

        var assignee = await userManager.FindByIdAsync(request.AssigneeId)
            ?? throw new NotFoundException(nameof(ApplicationUser), request.AssigneeId);

        task.AssigneeId = assignee.Id;
        task.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;

        await taskRepository.SaveChangesAsync(task, expectedRowVersion, cancellationToken);
        logger.LogInformation(
            "User {UserId} assigned task {TaskId} to {AssigneeId}",
            currentUser.Id,
            request.TaskId,
            assignee.Id);
    }
}
