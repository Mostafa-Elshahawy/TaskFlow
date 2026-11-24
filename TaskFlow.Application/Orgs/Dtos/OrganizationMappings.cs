using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Orgs.Dtos;

public static class OrganizationMappings
{
    public static OrganizationDto ToDto(this Organization organization)
    {
        return new OrganizationDto
        {
            Id = organization.Id,
            Name = organization.Name,
            Description = organization.Description,
            OwnerId = organization.OwnerId,
            CreatedAt = organization.CreatedAt
        };
    }
}
