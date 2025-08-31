using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Orgs.Commnads;

public class CreateOrgCommandHandler(ILogger<CreateOrgCommandHandler> logger, UserManager<ApplicationUser> userManager,
                                     IOrganizationRepository organizationRepository)
                                     : IRequestHandler<CreateOrgCommand, int>
{
    public async Task<int> Handle(CreateOrgCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating organization: {OrgName} with admin: {AdminEmail}", request.Name, request.AdminEmail);
        var adminUser = await userManager.FindByEmailAsync(request.AdminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = request.AdminEmail,
                Email = request.AdminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, request.AdminPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to create admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
        }

        var organization = new Organization
        {
            Name = request.Name,
            Description = request.Description,
            OwnerId = adminUser.Id,
            Members = new List<ApplicationUser> { adminUser },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            isDeleted = false
        };

        await organizationRepository.CreateOrganization(organization);
        logger.LogInformation("Organization created with ID: {OrgId}", organization.Id);
        return organization.Id;
    }
}
