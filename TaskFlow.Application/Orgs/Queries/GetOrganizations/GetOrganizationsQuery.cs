using MediatR;
using TaskFlow.Application.Orgs.Dtos;

namespace TaskFlow.Application.Orgs.Queries.GetOrganizations;

public record GetOrganizationsQuery : IRequest<IEnumerable<OrganizationDto>>;
