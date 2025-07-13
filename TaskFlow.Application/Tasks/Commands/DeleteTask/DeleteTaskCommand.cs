using MediatR;

namespace TaskFlow.Application.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand(int id) : IRequest
{
    public int Id { get; set; } = id;
}
