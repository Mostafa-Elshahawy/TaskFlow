using MediatR;

namespace TaskFlow.Application.Users.Commands.PromoteUser;

public record PromoteUserToManagerCommand(string UserId) : IRequest;
