using TaskFlow.Domain.Entites;
using TaskStatus = TaskFlow.Domain.Entites.TaskStatus;

namespace TaskFlow.Application.Tasks.Dtos;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TaskStatus Status { get; set; } = TaskStatus.Open;
    public TaskPriority Priority { get; set; } = TaskPriority.Low;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = default!;
    public string CreatedByUserId { get; set; } = default!;
    public string CreatedByName { get; set; } = default!;
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
