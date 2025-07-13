using MediatR;
using TaskFlow.Application.Tasks.Dtos;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Tasks.Queries.GetTasks;

public class GetTasksQuery : IRequest<List<TaskDto>>
{
    public int? ProjectId { get; set; }
    public Domain.Entites.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public string? AssigneeId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? DueBefore { get; set; }
    public DateTime? DueAfter { get; set; }
}
