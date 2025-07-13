using MediatR;

namespace TaskFlow.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Status { get; set; } = "Pending";
    public string Priority { get; set; } = "Normal";
    public string UpdatedByUserId { get; set; } = default!;
    public string UpdatedByName { get; set; } = default!;
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime DueDate { get; set; }
}

