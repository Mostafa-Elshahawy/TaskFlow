using MediatR;
using TaskFlow.Application.Users.Dtos;

namespace TaskFlow.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<IEnumerable<UserDto>>;
