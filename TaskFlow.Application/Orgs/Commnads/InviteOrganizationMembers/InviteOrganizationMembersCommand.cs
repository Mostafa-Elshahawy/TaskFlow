using MediatR;

namespace TaskFlow.Application.Orgs.Commnads.AddOrganizationMembers;

public record InviteOrganizationMembersCommand(int OrganizationId, string Email) : IRequest<int>
{
}
