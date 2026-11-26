using MediatR;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Tasks.Dtos;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Queries.GetTaskHistory;

public class GetTaskHistoryQueryHandler(
    ITaskRepository taskRepository,
    IWorkspaceAccessService accessService)
    : IRequestHandler<GetTaskHistoryQuery, IReadOnlyList<TaskStatusHistoryDto>>
{
    public async Task<IReadOnlyList<TaskStatusHistoryDto>> Handle(
        GetTaskHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetById(request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task", request.TaskId.ToString());

        await accessService.RequireProjectViewAsync(task.ProjectId, cancellationToken);
        var history = await taskRepository.GetStatusHistoryAsync(request.TaskId, cancellationToken);
        return history.Select(entry => entry.ToDto()).ToList();
    }
}
