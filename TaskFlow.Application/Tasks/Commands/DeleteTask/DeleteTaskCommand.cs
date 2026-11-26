using MediatR;

namespace TaskFlow.Application.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand(int id, string rowVersion) : IRequest
{
    public int Id { get; set; } = id;
    public string RowVersion { get; set; } = rowVersion;
}
