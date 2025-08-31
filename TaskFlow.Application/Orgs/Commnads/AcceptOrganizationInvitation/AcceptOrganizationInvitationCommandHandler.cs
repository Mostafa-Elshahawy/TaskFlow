using MediatR;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Orgs.Commnads.AcceptOrganizationInvitation;

public class AcceptOrganizationInvitationCommandHandler(IOrganizationRepository organizationRepository) 
                            : IRequestHandler<AcceptOrganizationInvitationCommand, Unit>
{
    public async Task<Unit> Handle(AcceptOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        var invite = await organizationRepository.GetInvitationByTokenAsync(request.Token, cancellationToken);

        if (invite is null || invite.Accepted)
            throw new InvalidOperationException("Invalid or expired invitation.");

        bool alreadyMember = invite.Organization.Members.Any(m => m.UserId == request.UserId);
        if (alreadyMember)
            throw new InvalidOperationException("User already a member of this organization.");

        var membership = new OrganizationMember
        {
            UserId = request.UserId,
            OrganizationId = invite.OrganizationId,
            Role = invite.Role
        };

        await organizationRepository.AddMemberAsync(membership);

        invite.Accepted = true;
        await organizationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
