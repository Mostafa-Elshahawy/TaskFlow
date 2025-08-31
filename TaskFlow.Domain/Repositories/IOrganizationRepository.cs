using TaskFlow.Domain.Entites;

namespace TaskFlow.Domain.Repositories;

public interface IOrganizationRepository
{
    Task<int> CreateOrganization(Organization organization);
    Task<Organization?> GetByIdAsync(int id);
    Task AddMemberAsync(OrganizationMember member);
    Task AddInvitationAsync(OrganizationInvitation invitation);
    Task<OrganizationInvitation?> GetInvitationByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
