using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

internal class OrganizationRepository(ApplicationDBContext dBContext) : IOrganizationRepository
{
    public async Task<int> CreateOrganization(Organization organization)
    {
        dBContext.Add(organization);
        await dBContext.SaveChangesAsync();
        return organization.Id;
    }
}
