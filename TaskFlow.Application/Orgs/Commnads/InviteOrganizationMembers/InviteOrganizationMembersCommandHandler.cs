using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Orgs.Commnads.AddOrganizationMembers;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Orgs.Commnads.InviteOrganizationMembers
{
    public class InviteOrganizationMembersCommandHandler(ILogger<InviteOrganizationMembersCommandHandler> logger, IOrganizationRepository organizationRepository,
                                         IEmailSender emailSender) : IRequestHandler<InviteOrganizationMembersCommand, int>

    {
        public async Task<int> Handle(InviteOrganizationMembersCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling Organization invitation for OrganizationId: {OrganizationId}, Email: {Email}", request.OrganizationId, request.Email);

            var org = await organizationRepository.GetByIdAsync(request.OrganizationId);
            if (org is null)
                throw new InvalidOperationException("Organization not found.");

            var invite = new OrganizationInvitation
            {
                OrganizationId = request.OrganizationId,
                InvitedEmail = request.Email,
                Token = Guid.NewGuid().ToString("N")
            };

            await organizationRepository.AddInvitationAsync(invite);
            await organizationRepository.SaveChangesAsync(cancellationToken);

            var acceptUrl = $"https://taskflow.com/accept-invite?token={invite.Token}";
            var body = $@"
            <p>You have been invited to join <b>{org.Name}</b>.</p>
            <p><a href='{acceptUrl}'>Click here to accept</a></p>";

            await emailSender.SendEmailAsync(request.Email, "Organization Invitation", body);
            return invite.Id;
        }
    }
}
