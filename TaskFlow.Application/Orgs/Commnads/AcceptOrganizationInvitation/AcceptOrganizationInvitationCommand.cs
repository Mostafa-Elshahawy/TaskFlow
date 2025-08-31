using MediatR;

namespace TaskFlow.Application.Orgs.Commnads.AcceptOrganizationInvitation
{
    public record AcceptOrganizationInvitationCommand(string UserId, string Token) : IRequest<Unit>;
}
