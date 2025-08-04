using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler(ILogger<CreateProjectCommandHandler> logger,IMapper mapper, IProjectRepository projectRepository, IUserContext userContext) : IRequestHandler<CreateProjectCommand, int>
{
    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        if (currentUser == null)
        {
            logger.LogWarning("Current user is null. Cannot create project.");
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        logger.LogInformation("User {User} is Creating a new project with Name: {ProjectName}",currentUser, request.Name);
        var project = mapper.Map<Project>(request);
        project.CreatedByUserId = currentUser.Id;
        int id = await projectRepository.CreateProjectAsync(project);
        logger.LogInformation("Project created with ID: {ProjectId}", id);
        return id;
    }
}
