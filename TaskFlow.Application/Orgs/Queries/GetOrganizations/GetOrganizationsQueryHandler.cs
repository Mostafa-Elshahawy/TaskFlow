using AutoMapper;
using MediatR;
using TaskFlow.Application.Orgs.Dtos;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Orgs.Queries.GetOrganizations;

public class GetOrganizationsQueryHandler(IOrganizationRepository organizationRepository, IMapper mapper)
    : IRequestHandler<GetOrganizationsQuery, IEnumerable<OrganizationDto>>
{
    public async Task<IEnumerable<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var orgs = await organizationRepository.GetAllAsync();
        return mapper.Map<IEnumerable<OrganizationDto>>(orgs);
    }
}
