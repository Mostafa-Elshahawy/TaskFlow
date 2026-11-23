using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Tasks.Dtos;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler(
    ILogger<GetTasksQueryHandler> logger,
    IMapper mapper,
    ITaskRepository taskRepository,
    IWorkspaceAccessService accessService) : IRequestHandler<GetTasksQuery, List<TaskDto>>
{
    public async Task<List<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching tasks with filters: {@Filters}", request);

        var filter = new TaskFilter
        {
            ProjectId = request.ProjectId,
            Status = request.Status,
            Priority = request.Priority,
            AssigneeId = request.AssigneeId,
            DueBefore = request.DueBefore,
            DueAfter = request.DueAfter
        };

        var currentUser = accessService.GetCurrentUser();
        var tasks = await taskRepository.GetFilteredTasksForUser(filter, currentUser.Id, cancellationToken);

        return mapper.Map<List<TaskDto>>(tasks);
    }
}
