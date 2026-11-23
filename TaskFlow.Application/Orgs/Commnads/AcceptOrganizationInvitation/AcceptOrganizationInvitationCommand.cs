using MediatR;

namespace TaskFlow.Application.Orgs.Commnads.AcceptOrganizationInvitation;

public record AcceptOrganizationInvitationCommand(string Token) : IRequest<Unit>;
