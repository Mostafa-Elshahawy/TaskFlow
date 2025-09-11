using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Commands.AddMemebers;

public class AddMemebersCommandHandler(ILogger<AddMemebersCommandHandler> logger, IUserContext userContext, UserManager<ApplicationUser> userManager,
             IProjectRepository projectRepository): IRequestHandler<AddMemebersCommand>
{
    public async Task Handle(AddMemebersCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        logger.LogInformation("{UserEmail} [{UserId}] is adding members to project with ID {ProjectId}",
            currentUser?.Email,
            currentUser?.Id,
            request.ProjectId);

        if (currentUser == null)
        {
            logger.LogWarning("User with ID {UserId} not found.", request.UserId);
            throw new KeyNotFoundException("User not found.");
        }

        if (!currentUser.Roles.Contains("Manager"))
            throw new UnauthorizedAccessException("Only managers can add members to projects.");

        var project =  await projectRepository.GetProjectByIdAsync(request.ProjectId);
        if (project == null || project.isDeleted)
            throw new NotFoundException(nameof(project), request.ProjectId.ToString());

        var projectMember = new ProjectMember
        {
            ProjectId = project.Id,
            UserId = request.UserId,
            AddedAt = DateTime.UtcNow
        };

        project.Members.Add(projectMember);
        project.UpdatedAt = DateTime.UtcNow;

        await projectRepository.UpdateAsync(project);
    }
}
