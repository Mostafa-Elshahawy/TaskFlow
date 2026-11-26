using TaskFlow.Domain.Entites;
using TaskStatus = TaskFlow.Domain.Entites.TaskStatus;

namespace TaskFlow.Application.Tasks.Dtos;

public class TaskStatusHistoryDto
{
    public long Id { get; set; }
    public TaskStatus FromStatus { get; set; }
    public TaskStatus ToStatus { get; set; }
    public string ChangedByUserId { get; set; } = default!;
    public string? ChangedByName { get; set; }
    public DateTimeOffset ChangedAtUtc { get; set; }
}
