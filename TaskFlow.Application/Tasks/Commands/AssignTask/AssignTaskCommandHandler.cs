using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.AssignTask;

public class AssignTaskCommandHandler(ILogger<AssignTaskCommandHandler> logger, ITaskRepository taskRepository, 
    UserManager<ApplicationUser> userManager, IUserContext userContext) : IRequestHandler<AssignTaskCommand>
{
    public async Task Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        logger.LogInformation("{UserEmail} [{UserId}] is assigning task with ID {TaskId} to user with ID {AssigneeId}", 
            currentUser?.Email, 
            currentUser?.Id, 
            request.TaskId, 
            request.AssigneeId);

        if (currentUser == null)
        {
            logger.LogWarning("User with ID {UserId} not found.", request.AssignedById);
            throw new NotFoundException(nameof(currentUser), request.AssignedById.ToString());
        }

        if (!currentUser.Roles.Contains("Manager"))
            throw new UnauthorizedAccessException("Only managers can assign tasks.");

        var task = await taskRepository.GetById(request.TaskId);
        if (task == null || task.isDeleted)
            throw new KeyNotFoundException("Task not found.");

        var assignee = await userManager.FindByIdAsync(request.AssigneeId);
        if (assignee == null)
            throw new NotFoundException(nameof(assignee),request.AssigneeId.ToString());

        task.AssigneeId = request.AssigneeId;
        task.UpdatedAt = DateTime.UtcNow;

        await taskRepository.AssignTask(task);
        logger.LogInformation("Task with ID {TaskId} assigned to user with ID {UserId}.", request.TaskId, request.AssigneeId);
    }
}
