using MediatR;

namespace TaskFlow.Application.Tasks.Commands.AssignTask;

public class AssignTaskCommand : IRequest
{
    public string AssignedById { get; set; } = default!;
    public string AssigneeId { get; set; } = default!;
    public int TaskId { get; set; }
}
