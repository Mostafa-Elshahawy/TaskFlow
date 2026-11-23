using MediatR;
using TaskFlow.Application.Orgs.Invitations;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Orgs.Commnads.AcceptOrganizationInvitation;

public class AcceptOrganizationInvitationCommandHandler(
    IOrganizationRepository organizationRepository,
    IInvitationTokenService tokenService,
    IUserContext userContext,
    TimeProvider timeProvider) : IRequestHandler<AcceptOrganizationInvitationCommand, Unit>
{
    public async Task<Unit> Handle(
        AcceptOrganizationInvitationCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()
            ?? throw new UnauthorizedAccessException("Authentication is required to accept an invitation.");
        var tokenHash = tokenService.HashToken(request.Token);
        var invitation = await organizationRepository.GetInvitationByTokenHashAsync(
                tokenHash,
                cancellationToken)
            ?? throw new InvalidInvitationException();
        var now = timeProvider.GetUtcNow().UtcDateTime;

        if (invitation.AcceptedAtUtc is not null)
        {
            if (invitation.AcceptedByUserId == currentUser.Id)
                return Unit.Value;

            throw new InvalidInvitationException();
        }

        if (invitation.RevokedAtUtc is not null || invitation.ExpiresAtUtc <= now)
            throw new InvalidInvitationException();

        var normalizedCurrentEmail = currentUser.Email.Trim().ToUpperInvariant();
        if (!string.Equals(invitation.InvitedEmail, normalizedCurrentEmail, StringComparison.Ordinal))
            throw new ForbidException();

        var alreadyMember = invitation.Organization.Members.Any(member => member.UserId == currentUser.Id);
        if (!alreadyMember)
        {
            await organizationRepository.AddMemberAsync(
                new OrganizationMember
                {
                    UserId = currentUser.Id,
                    OrganizationId = invitation.OrganizationId,
                    Role = invitation.Role,
                    JoinedAt = now
                },
                cancellationToken);
        }

        invitation.AcceptedAtUtc = now;
        invitation.AcceptedByUserId = currentUser.Id;
        await organizationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
