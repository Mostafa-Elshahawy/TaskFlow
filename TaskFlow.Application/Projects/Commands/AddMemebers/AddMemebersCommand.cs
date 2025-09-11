using MediatR;

namespace TaskFlow.Application.Projects.Commands.AddMemebers;

public class AddMemebersCommand : IRequest
{
    public int ProjectId { get; set; }
    public string UserId { get; set; } = default!;
}
