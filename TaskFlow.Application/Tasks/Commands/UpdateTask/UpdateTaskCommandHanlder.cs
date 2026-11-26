using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Common;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHanlder(
    ILogger<UpdateTaskCommandHanlder> logger,
    ITaskRepository taskRepository,
    IWorkspaceAccessService accessService,
    TimeProvider timeProvider) : IRequestHandler<UpdateTaskCommand>
{
    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var expectedRowVersion = ConcurrencyToken.Decode(request.RowVersion);
        var task = await taskRepository.GetByIdForMutation(request.Id, cancellationToken)
            ?? throw new NotFoundException("Task", request.Id.ToString());

        if (!Enum.IsDefined(request.Status))
            throw new ArgumentException($"Unsupported task status '{request.Status}'.");

        if (!Enum.IsDefined(request.Priority))
            throw new ArgumentException($"Unsupported task priority '{request.Priority}'.");

        await accessService.RequireProjectContributionAsync(task.Project, cancellationToken);
        if (task.isDeleted || task.Project.isDeleted)
            throw new ConflictException("The task is no longer available. Reload and try again.");

        var currentUser = accessService.GetCurrentUser();
        var now = timeProvider.GetUtcNow();
        var previousStatus = task.Status;

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.UpdatedAt = now.UtcDateTime;

        if (previousStatus != request.Status)
        {
            task.StatusHistory.Add(new TaskStatusHistory
            {
                TaskId = task.Id,
                FromStatus = previousStatus,
                ToStatus = request.Status,
                ChangedByUserId = currentUser.Id,
                ChangedAtUtc = now
            });

            task.CompletedAt = request.Status == TaskFlow.Domain.Entites.TaskStatus.Completed
                ? now.UtcDateTime
                : null;
        }

        await taskRepository.SaveChangesAsync(task, expectedRowVersion, cancellationToken);
        logger.LogInformation("Updated task with ID {TaskId}", request.Id);
    }
}
