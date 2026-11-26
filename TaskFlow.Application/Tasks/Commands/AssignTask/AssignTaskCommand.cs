using MediatR;

namespace TaskFlow.Application.Tasks.Commands.AssignTask;

public class AssignTaskCommand : IRequest
{
    public string AssigneeId { get; set; } = default!;
    public int TaskId { get; set; }
    public string RowVersion { get; set; } = default!;
}
