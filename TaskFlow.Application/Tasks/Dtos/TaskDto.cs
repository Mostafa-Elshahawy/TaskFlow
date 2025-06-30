namespace TaskFlow.Application.Tasks.Dtos;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Priority { get; set; } = default!;
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
