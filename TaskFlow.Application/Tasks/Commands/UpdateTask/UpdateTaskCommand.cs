using MediatR;
using TaskFlow.Domain.Entites;
using DomainTaskStatus = TaskFlow.Domain.Entites.TaskStatus;

namespace TaskFlow.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DomainTaskStatus Status { get; set; } = DomainTaskStatus.Open;
    public TaskPriority Priority { get; set; } = TaskPriority.Low;
    public DateTime DueDate { get; set; }
}
