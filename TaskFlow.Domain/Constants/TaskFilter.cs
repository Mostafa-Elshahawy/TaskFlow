using TaskFlow.Domain.Entites;

namespace TaskFlow.Domain.Constants;

public class TaskFilter
{
    public Entites.TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? AssigneeId { get; set; }
    public int? ProjectId { get; set; }
    public int? OrganizationId { get; set; }
    public DateTime? DueBefore { get; set; }
    public DateTime? DueAfter { get; set; }
}

