using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Orgs.Commnads.CreateOrganization;

public class CreateOrgCommandHandler(
    ILogger<CreateOrgCommandHandler> logger,
    IOrganizationRepository organizationRepository,
    IUserContext userContext)
                                     : IRequestHandler<CreateOrgCommand, int>
{
    public async Task<int> Handle(CreateOrgCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        logger.LogInformation(
            "User {UserId} is creating organization {OrganizationName}",
            currentUser.Id,
            request.Name);

        var organization = new Organization
        {
            Name = request.Name,
            Description = request.Description,
            OwnerId = currentUser.Id,
            Members = new List<OrganizationMember>
            {
                new OrganizationMember
                {
                    UserId = currentUser.Id,
                    Role = OrganizationRole.Owner,
                    JoinedAt = DateTime.UtcNow
                }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            isDeleted = false
        };

        await organizationRepository.CreateOrganization(organization);
        logger.LogInformation("Organization created with ID: {OrgId}", organization.Id);
        return organization.Id;
    }
}
