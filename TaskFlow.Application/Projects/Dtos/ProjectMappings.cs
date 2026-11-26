using TaskFlow.Application.Projects.Commands.CreateProject;
using TaskFlow.Application.Common;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Projects.Dtos;

public static class ProjectMappings
{
    public static Project ToEntity(
        this CreateProjectCommand command,
        string createdByUserId,
        DateTime createdAtUtc)
    {
        return new Project
        {
            Name = command.Name,
            Description = command.Description,
            CreatedByUserId = createdByUserId,
            OrganizationId = command.OrganizationId,
            CreatedAt = createdAtUtc,
            UpdatedAt = null,
            isDeleted = false
        };
    }

    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedByUserId = project.CreatedByUserId,
            CreatedByName = project.CreatedBy?.UserName!,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            IsDeleted = project.isDeleted,
            OrganizationId = project.OrganizationId,
            RowVersion = ConcurrencyToken.Encode(project.RowVersion)
        };
    }
}
