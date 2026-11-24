using MediatR;
using TaskFlow.Application.Orgs.Dtos;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Orgs.Queries.GetOrganizations;

public class GetOrganizationsQueryHandler(
    IOrganizationRepository organizationRepository,
    IUserContext userContext)
    : IRequestHandler<GetOrganizationsQuery, IEnumerable<OrganizationDto>>
{
    public async Task<IEnumerable<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var orgs = await organizationRepository.GetAllForUserAsync(currentUser.Id, cancellationToken);
        return orgs.Select(organization => organization.ToDto());
    }
}
