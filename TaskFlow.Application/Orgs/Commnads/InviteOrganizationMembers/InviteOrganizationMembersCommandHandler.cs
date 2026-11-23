using System.Text.Encodings.Web;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Orgs.Commnads.AddOrganizationMembers;
using TaskFlow.Application.Orgs.Invitations;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Orgs.Commnads.InviteOrganizationMembers;

public class InviteOrganizationMembersCommandHandler(
    ILogger<InviteOrganizationMembersCommandHandler> logger,
    IOrganizationRepository organizationRepository,
    IEmailOutboxRepository emailOutboxRepository,
    IInvitationTokenService tokenService,
    IWorkspaceAccessService accessService,
    IOptions<InvitationOptions> invitationOptions,
    TimeProvider timeProvider) : IRequestHandler<InviteOrganizationMembersCommand, int>
{
    public async Task<int> Handle(
        InviteOrganizationMembersCommand request,
        CancellationToken cancellationToken)
    {
        await accessService.RequireOrganizationManagementAsync(request.OrganizationId, cancellationToken);

        var organization = await organizationRepository.GetByIdAsync(
                request.OrganizationId,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Organization), request.OrganizationId.ToString());

        var deliveryEmail = request.Email.Trim();
        var normalizedEmail = deliveryEmail.ToUpperInvariant();
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var options = invitationOptions.Value;

        if (options.LifetimeHours <= 0)
            throw new ArgumentException("Invitation lifetime must be greater than zero.");

        if (!Uri.TryCreate(options.ClientBaseUrl, UriKind.Absolute, out var clientBaseUri))
            throw new ArgumentException("Invitation client base URL must be absolute.");

        var outstanding = await organizationRepository.GetOutstandingInvitationAsync(
            request.OrganizationId,
            normalizedEmail,
            cancellationToken);

        if (outstanding is not null)
        {
            if (outstanding.ExpiresAtUtc > now)
                throw new ConflictException("An active invitation already exists for this email address.");

            outstanding.RevokedAtUtc = now;
        }

        var token = tokenService.CreateToken();
        var invitation = new OrganizationInvitation
        {
            OrganizationId = request.OrganizationId,
            InvitedEmail = normalizedEmail,
            TokenHash = token.Hash,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddHours(options.LifetimeHours)
        };

        await organizationRepository.AddInvitationAsync(invitation, cancellationToken);

        var invitationPath = $"accept-invite?token={Uri.EscapeDataString(token.RawToken)}";
        var acceptUrl = new Uri(
            new Uri(clientBaseUri.ToString().TrimEnd('/') + "/"),
            invitationPath);
        var encodedOrganizationName = HtmlEncoder.Default.Encode(organization.Name);
        var encodedAcceptUrl = HtmlEncoder.Default.Encode(acceptUrl.ToString());
        var body = $"<p>You have been invited to join <b>{encodedOrganizationName}</b>.</p>" +
                   $"<p><a href='{encodedAcceptUrl}'>Accept invitation</a></p>";

        await emailOutboxRepository.EnqueueAsync(
            deliveryEmail,
            "Organization invitation",
            body,
            cancellationToken);
        await organizationRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Created organization invitation {InvitationId} for OrganizationId {OrganizationId}.",
            invitation.Id,
            invitation.OrganizationId);

        return invitation.Id;
    }
}
