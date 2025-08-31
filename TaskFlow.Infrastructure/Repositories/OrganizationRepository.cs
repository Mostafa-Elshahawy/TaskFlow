using AutoMapper.Execution;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

internal class OrganizationRepository(ApplicationDBContext dBContext) : IOrganizationRepository
{
    public async Task AddInvitationAsync(OrganizationInvitation invitation)
    {
        await dBContext.OrganizationInvitations.AddAsync(invitation);
    }

    public async Task AddMemberAsync(OrganizationMember member)
    {
        await dBContext.OrganizationMembers.AddAsync(member);
    }

    public async Task<int> CreateOrganization(Organization organization)
    {
        dBContext.Add(organization);
        await dBContext.SaveChangesAsync();
        return organization.Id;
    }

    public async Task<Organization?> GetByIdAsync(int id)
    {
        return await dBContext.Organizations
          .Include(o => o.Members)
          .FirstOrDefaultAsync(o => o.Id == id && !o.isDeleted);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dBContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrganizationInvitation?> GetInvitationByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await dBContext.OrganizationInvitations
            .Include(i => i.Organization)
            .FirstOrDefaultAsync(i => i.Token == token, cancellationToken);
    }
}
