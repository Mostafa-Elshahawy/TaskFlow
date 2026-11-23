using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler(
    ILogger<DeleteTaskCommandHandler> logger,
    ITaskRepository taskRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<DeleteTaskCommand>
{
    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.Id, cancellationToken)
            ?? throw new NotFoundException("Task", request.Id.ToString());

        await accessService.RequireProjectManagementAsync(task.ProjectId, cancellationToken);

        logger.LogInformation("Deleting task with ID {TaskId}", request.Id);
        await taskRepository.Delete(task);
    }
}
