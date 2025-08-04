using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Projects.Dtos;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler(ILogger<GetProjectByIdQueryHandler> logger, IMapper mapper, IProjectRepository project) : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching project with ID: {ProjectId}", request.Id);
        
        var projectEntity = await project.GetProjectByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(project), request.Id.ToString());

        var projectDto = mapper.Map<ProjectDto>(projectEntity);
        logger.LogInformation("Project fetched successfully: {@Project}", projectDto);
        
        return projectDto;
    }
}
