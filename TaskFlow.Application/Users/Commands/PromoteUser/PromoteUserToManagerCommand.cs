using MediatR;

namespace TaskFlow.Application.Users.Commands.PromoteUser;

public record PromoteUserToManagerCommand(string userId, int projectId) : IRequest;
