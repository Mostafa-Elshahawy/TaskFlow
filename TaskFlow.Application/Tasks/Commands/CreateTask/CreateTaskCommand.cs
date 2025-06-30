using MediatR;

namespace TaskFlow.Application.Tasks.Commands.CreateTask;

public class CreateTaskCommand : IRequest<int>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Status { get; set; } = "Pending"; 
    public string Priority { get; set; } = "Normal";
    public int ProjectId { get; set; }
    public string CreatedByUserId { get; set; } = default!;
    public string CreatedByName { get; set; } = default!;
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime DueDate { get; set; }
}
