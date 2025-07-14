using MediatR;
using TaskFlow.Application.Projects.Dtos;

namespace TaskFlow.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQuery(int id) : IRequest<ProjectDto?>
{
    public int Id { get; } = id;
}