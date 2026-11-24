using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Tasks.Dtos;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler(
    ILogger<GetTaskByIdQueryHandler> logger,
    ITaskRepository taskRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting task with ID: {TaskId}", request.Id);

        var task = await taskRepository.GetById(request.Id, cancellationToken)
            ?? throw new NotFoundException("Task", request.Id.ToString());

        await accessService.RequireProjectViewAsync(task.ProjectId, cancellationToken);
        return task.ToDto();
    }
}
