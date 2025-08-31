using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Users.Commands.PromoteUser;

public class PromoteUserToManagerCommandHandler(ILogger<PromoteUserToManagerCommandHandler> logger, UserManager<ApplicationUser> userManager,
                                                 IProjectRepository projectRepository)
                                : IRequestHandler<PromoteUserToManagerCommand>
{
    public async Task Handle(PromoteUserToManagerCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.userId);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found.", request.userId);
            throw new NotFoundException(nameof(user), request.userId.ToString());
        }

        var isInRole = await userManager.IsInRoleAsync(user, "Manager");
        if (isInRole)
        {
            logger.LogInformation("User with ID {UserId} is already a Manager.", request.userId);
            return;
        }

        var project = await projectRepository.GetProjectByIdAsync(request.projectId);
        if (project is null)
        {
            throw new NotFoundException(nameof(project),request.projectId.ToString());
        }

        var result = await userManager.AddToRoleAsync(user, UserRoles.Manager);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                    $"Failed to promote user {user.Email} to Manager: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await projectRepository.AssignManagerToProject(user, project);

        logger.LogInformation("User with ID {UserId} has been promoted to Manager.", request.userId);
    }
}
