namespace TaskFlow.Domain.Entites;

public class TaskStatusHistory
{
    public long Id { get; set; }
    public int TaskId { get; set; }
    public TaskEntity Task { get; set; } = default!;
    public TaskStatus FromStatus { get; set; }
    public TaskStatus ToStatus { get; set; }
    public string ChangedByUserId { get; set; } = default!;
    public ApplicationUser ChangedBy { get; set; } = default!;
    public DateTimeOffset ChangedAtUtc { get; set; }
}
