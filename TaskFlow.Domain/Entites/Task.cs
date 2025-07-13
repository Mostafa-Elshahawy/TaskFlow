using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Domain.Entites;

public class TaskEntity
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Title { get; set; } = default!;

    [MaxLength(500)]
    public string Description { get; set; } = default!;
    public TaskStatus Status { get; set; } = TaskStatus.Open;
    public TaskPriority Priority { get; set; } 
    public int ProjectId { get; set; }
    public Project Project { get; set; } = default!;
    public string CreatedByUserId { get; set; } = default!;
    public string? AssigneeId { get; set; }
    public ApplicationUser? Assignee { get; set; }
    public ApplicationUser CreatedBy { get; set; } = default!;
    public DateTime DueDate { get; set; } = default!;
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool isDeleted { get; set; }
}

public enum TaskStatus
{
    Open,
    InProgress,
    Completed,
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
}