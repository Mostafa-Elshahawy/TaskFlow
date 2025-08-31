using TaskFlow.Domain.Entites;

namespace TaskFlow.Domain.Repositories;

public interface IOrganizationRepository
{
    Task<int> CreateOrganization(Organization organization);
}
