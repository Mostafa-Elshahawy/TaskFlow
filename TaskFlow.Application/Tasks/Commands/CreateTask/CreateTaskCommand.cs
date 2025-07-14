using MediatR;
using TaskFlow.Domain.Entites;
using TaskStatus = TaskFlow.Domain.Entites.TaskStatus;

namespace TaskFlow.Application.Tasks.Commands.CreateTask;

public class CreateTaskCommand : IRequest<int>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TaskStatus Status { get; set; } = TaskStatus.Open; 
    public TaskPriority Priority{ get; set; } = TaskPriority.Low;
    public int ProjectId { get; set; }
    public string CreatedByUserId { get; set; } = default!;
    public string CreatedByName { get; set; } = default!;
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime DueDate { get; set; }
}
