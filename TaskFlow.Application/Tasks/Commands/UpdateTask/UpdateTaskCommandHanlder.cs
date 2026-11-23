using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHanlder(
    ILogger<UpdateTaskCommandHanlder> logger,
    ITaskRepository taskRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<UpdateTaskCommand>
{
    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.Id, cancellationToken)
            ?? throw new NotFoundException("Task", request.Id.ToString());

        if (!Enum.IsDefined(request.Status))
            throw new ArgumentException($"Unsupported task status '{request.Status}'.");

        if (!Enum.IsDefined(request.Priority))
            throw new ArgumentException($"Unsupported task priority '{request.Priority}'.");

        await accessService.RequireProjectContributionAsync(task.ProjectId, cancellationToken);

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await taskRepository.SaveChanges();
        logger.LogInformation("Updated task with ID {TaskId}", request.Id);
    }
}
