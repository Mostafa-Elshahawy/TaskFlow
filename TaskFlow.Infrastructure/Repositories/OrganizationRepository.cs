using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

internal class OrganizationRepository(ApplicationDBContext dBContext) : IOrganizationRepository
{
    public async Task AddInvitationAsync(OrganizationInvitation invitation, CancellationToken cancellationToken = default)
    {
        await dBContext.OrganizationInvitations.AddAsync(invitation, cancellationToken);
    }

    public async Task AddMemberAsync(OrganizationMember member, CancellationToken cancellationToken = default)
    {
        await dBContext.OrganizationMembers.AddAsync(member, cancellationToken);
    }

    public async Task<int> CreateOrganization(Organization organization)
    {
        dBContext.Add(organization);
        await dBContext.SaveChangesAsync();
        return organization.Id;
    }

    public async Task<IEnumerable<Organization>> GetAllForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await dBContext.Organizations
            .AsNoTracking()
            .Where(organization => organization.OwnerId == userId ||
                                   organization.Members.Any(member => member.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<Organization?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dBContext.Organizations
          .Include(o => o.Members)
          .FirstOrDefaultAsync(o => o.Id == id && !o.isDeleted, cancellationToken);
    }

    public async Task<OrganizationRole?> GetMemberRoleAsync(
        int organizationId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var isOwner = await dBContext.Organizations
            .AsNoTracking()
            .AnyAsync(
                organization => organization.Id == organizationId && organization.OwnerId == userId,
                cancellationToken);

        if (isOwner)
            return OrganizationRole.Owner;

        return await dBContext.OrganizationMembers
            .AsNoTracking()
            .Where(member => member.OrganizationId == organizationId &&
                             member.UserId == userId &&
                             !member.Organization.isDeleted)
            .Select(member => (OrganizationRole?)member.Role)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dBContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConflictException("The invitation was changed by another request.", exception);
        }
        catch (DbUpdateException exception)
        {
            throw new ConflictException("The requested organization change conflicts with existing data.", exception);
        }
    }

    public async Task<OrganizationInvitation?> GetOutstandingInvitationAsync(
        int organizationId,
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return await dBContext.OrganizationInvitations
            .FirstOrDefaultAsync(
                invitation =>
                    invitation.OrganizationId == organizationId &&
                    invitation.InvitedEmail == normalizedEmail &&
                    invitation.AcceptedAtUtc == null &&
                    invitation.RevokedAtUtc == null,
                cancellationToken);
    }

    public async Task<OrganizationInvitation?> GetInvitationByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await dBContext.OrganizationInvitations
            .Include(invitation => invitation.Organization)
            .ThenInclude(organization => organization.Members)
            .FirstOrDefaultAsync(invitation => invitation.TokenHash == tokenHash, cancellationToken);
    }
}
