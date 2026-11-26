using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Common;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler(
    ILogger<DeleteTaskCommandHandler> logger,
    ITaskRepository taskRepository,
    IWorkspaceAccessService accessService,
    TimeProvider timeProvider) : IRequestHandler<DeleteTaskCommand>
{
    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var expectedRowVersion = ConcurrencyToken.Decode(request.RowVersion);
        var task = await taskRepository.GetByIdForMutation(request.Id, cancellationToken)
            ?? throw new NotFoundException("Task", request.Id.ToString());

        await accessService.RequireProjectManagementAsync(task.Project, cancellationToken);
        if (task.isDeleted || task.Project.isDeleted)
            throw new ConflictException("The task is no longer available. Reload and try again.");

        logger.LogInformation("Deleting task with ID {TaskId}", request.Id);
        task.isDeleted = true;
        task.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
        await taskRepository.SaveChangesAsync(task, expectedRowVersion, cancellationToken);
    }
}
