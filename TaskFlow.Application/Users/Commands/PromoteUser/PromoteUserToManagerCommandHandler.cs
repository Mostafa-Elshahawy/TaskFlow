using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Application.Users.Commands.PromoteUser;

public class PromoteUserToManagerCommandHandler(
    ILogger<PromoteUserToManagerCommandHandler> logger,
    UserManager<ApplicationUser> userManager) : IRequestHandler<PromoteUserToManagerCommand>
{
    public async Task Handle(PromoteUserToManagerCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);

        if (await userManager.IsInRoleAsync(user, UserRoles.Manager))
        {
            logger.LogInformation("User {UserId} is already a manager", request.UserId);
            return;
        }

        var result = await userManager.AddToRoleAsync(user, UserRoles.Manager);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to promote user {user.Email}: {string.Join(", ", result.Errors.Select(error => error.Description))}");
        }

        logger.LogInformation("Promoted user {UserId} to global manager", request.UserId);
    }
}
