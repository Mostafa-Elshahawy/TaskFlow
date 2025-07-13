using MediatR;
using TaskFlow.Application.Tasks.Dtos;

namespace TaskFlow.Application.Tasks.Queries.GetTaskById;

public class GetTaskByIdQuery(int id) : IRequest<TaskDto>
{
    public int Id { get; } = id;
}
