using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler(ILogger<DeleteProjectCommandHandler> logger, IProjectRepository projectRepository) : IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting project with ID: {ProjectId}", request.Id);
        
        var project = await projectRepository.GetProjectByIdAsync(request.Id);
        if (project == null)
        {
            logger.LogWarning("Project with ID: {ProjectId} not found", request.Id);
            throw new NotFoundException(nameof(project), request.Id.ToString());
        }
        await projectRepository.DeleteProjectAsync(project);
        logger.LogInformation("Project with ID: {ProjectId} deleted successfully", request.Id);
    }
}
