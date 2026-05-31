using MediatR;
using TaskFlow.Application.Tasks.Dtos;
using TaskStatus = TaskFlow.Domain.Entites.TaskStatus;
using TaskPriority = TaskFlow.Domain.Entites.TaskPriority;

namespace TaskFlow.Application.Tasks.Queries.GetTasks;

public class GetTasksQuery : IRequest<List<TaskDto>>
{
    public int? ProjectId { get; set; }
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? AssigneeId { get; set; }
    public DateTime? DueBefore { get; set; }
    public DateTime? DueAfter { get; set; }
}
