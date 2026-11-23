using TaskFlow.Domain.Entites;

namespace TaskFlow.Domain.Repositories;

public interface IOrganizationRepository
{
    Task<int> CreateOrganization(Organization organization);
    Task<IEnumerable<Organization>> GetAllForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Organization?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrganizationRole?> GetMemberRoleAsync(int organizationId, string userId, CancellationToken cancellationToken = default);
    Task AddMemberAsync(OrganizationMember member, CancellationToken cancellationToken = default);
    Task AddInvitationAsync(OrganizationInvitation invitation, CancellationToken cancellationToken = default);
    Task<OrganizationInvitation?> GetOutstandingInvitationAsync(int organizationId, string normalizedEmail, CancellationToken cancellationToken = default);
    Task<OrganizationInvitation?> GetInvitationByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
