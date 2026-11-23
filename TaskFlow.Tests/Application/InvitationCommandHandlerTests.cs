using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Orgs.Commnads.AcceptOrganizationInvitation;
using TaskFlow.Application.Orgs.Commnads.AddOrganizationMembers;
using TaskFlow.Application.Orgs.Commnads.InviteOrganizationMembers;
using TaskFlow.Application.Orgs.Invitations;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application;

public class InvitationCommandHandlerTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task InviteHandler_NormalizesEmailHashesTokenAndQueuesDurableEmail()
    {
        var repository = new FakeOrganizationRepository
        {
            Organization = new Organization { Id = 7, Name = "Platform" }
        };
        var outbox = new RecordingOutbox();
        var tokenService = new InvitationTokenService();
        var handler = CreateInviteHandler(repository, outbox, tokenService);

        await handler.Handle(
            new InviteOrganizationMembersCommand(7, " Member@Example.com "),
            CancellationToken.None);

        var invitation = Assert.Single(repository.AddedInvitations);
        Assert.Equal("MEMBER@EXAMPLE.COM", invitation.InvitedEmail);
        Assert.Equal(Now.UtcDateTime, invitation.CreatedAtUtc);
        Assert.Equal(Now.AddHours(24).UtcDateTime, invitation.ExpiresAtUtc);
        Assert.Equal(64, invitation.TokenHash.Length);
        Assert.Equal(1, repository.SaveCount);

        var email = Assert.Single(outbox.Messages);
        Assert.Equal("Member@Example.com", email.Recipient);
        var rawToken = ExtractToken(email.HtmlBody);
        Assert.Equal(invitation.TokenHash, tokenService.HashToken(rawToken));
        Assert.Contains("https://client.taskflow.test/accept-invite?token=", email.HtmlBody);
    }

    [Fact]
    public async Task InviteHandler_WhenOutstandingInviteIsValid_ThrowsConflict()
    {
        var repository = new FakeOrganizationRepository
        {
            Organization = new Organization { Id = 7, Name = "Platform" },
            OutstandingInvitation = Invitation(expiresAt: Now.AddHours(1))
        };
        var handler = CreateInviteHandler(repository, new RecordingOutbox(), new InvitationTokenService());

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new InviteOrganizationMembersCommand(7, "member@example.com"),
            CancellationToken.None));

        Assert.Empty(repository.AddedInvitations);
    }

    [Fact]
    public async Task InviteHandler_WhenOutstandingInviteExpired_RevokesAndReplacesIt()
    {
        var expired = Invitation(expiresAt: Now.AddMinutes(-1));
        var repository = new FakeOrganizationRepository
        {
            Organization = new Organization { Id = 7, Name = "Platform" },
            OutstandingInvitation = expired
        };
        var handler = CreateInviteHandler(repository, new RecordingOutbox(), new InvitationTokenService());

        await handler.Handle(
            new InviteOrganizationMembersCommand(7, "member@example.com"),
            CancellationToken.None);

        Assert.Equal(Now.UtcDateTime, expired.RevokedAtUtc);
        Assert.Single(repository.AddedInvitations);
    }

    [Fact]
    public async Task AcceptHandler_WithMatchingEmail_AddsMemberAndMarksInvitationAccepted()
    {
        const string rawToken = "valid-invitation-token";
        var tokenService = new InvitationTokenService();
        var invitation = Invitation(expiresAt: Now.AddHours(1));
        invitation.TokenHash = tokenService.HashToken(rawToken);
        var repository = new FakeOrganizationRepository { InvitationByHash = invitation };
        var handler = CreateAcceptHandler(repository, tokenService, "new-user", "member@example.com");

        await handler.Handle(new AcceptOrganizationInvitationCommand(rawToken), CancellationToken.None);

        var member = Assert.Single(repository.AddedMembers);
        Assert.Equal("new-user", member.UserId);
        Assert.Equal(invitation.OrganizationId, member.OrganizationId);
        Assert.Equal(Now.UtcDateTime, invitation.AcceptedAtUtc);
        Assert.Equal("new-user", invitation.AcceptedByUserId);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task AcceptHandler_WithWrongEmail_ThrowsForbidWithoutMutation()
    {
        const string rawToken = "valid-invitation-token";
        var tokenService = new InvitationTokenService();
        var invitation = Invitation(expiresAt: Now.AddHours(1));
        invitation.TokenHash = tokenService.HashToken(rawToken);
        var repository = new FakeOrganizationRepository { InvitationByHash = invitation };
        var handler = CreateAcceptHandler(repository, tokenService, "wrong-user", "wrong@example.com");

        await Assert.ThrowsAsync<ForbidException>(() => handler.Handle(
            new AcceptOrganizationInvitationCommand(rawToken),
            CancellationToken.None));

        Assert.Empty(repository.AddedMembers);
        Assert.Equal(0, repository.SaveCount);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task AcceptHandler_WithExpiredOrRevokedInvitation_ThrowsInvalidInvitation(
        bool expired,
        bool revoked)
    {
        const string rawToken = "valid-invitation-token";
        var tokenService = new InvitationTokenService();
        var invitation = Invitation(expiresAt: expired ? Now.AddSeconds(-1) : Now.AddHours(1));
        invitation.TokenHash = tokenService.HashToken(rawToken);
        invitation.RevokedAtUtc = revoked ? Now.AddMinutes(-1).UtcDateTime : null;
        var repository = new FakeOrganizationRepository { InvitationByHash = invitation };
        var handler = CreateAcceptHandler(repository, tokenService, "new-user", "member@example.com");

        await Assert.ThrowsAsync<InvalidInvitationException>(() => handler.Handle(
            new AcceptOrganizationInvitationCommand(rawToken),
            CancellationToken.None));

        Assert.Empty(repository.AddedMembers);
    }

    [Fact]
    public async Task AcceptHandler_WhenSameUserRepeatsAcceptance_IsIdempotent()
    {
        const string rawToken = "valid-invitation-token";
        var tokenService = new InvitationTokenService();
        var invitation = Invitation(expiresAt: Now.AddHours(1));
        invitation.TokenHash = tokenService.HashToken(rawToken);
        invitation.AcceptedAtUtc = Now.AddMinutes(-5).UtcDateTime;
        invitation.AcceptedByUserId = "existing-user";
        var repository = new FakeOrganizationRepository { InvitationByHash = invitation };
        var handler = CreateAcceptHandler(repository, tokenService, "existing-user", "member@example.com");

        await handler.Handle(new AcceptOrganizationInvitationCommand(rawToken), CancellationToken.None);

        Assert.Empty(repository.AddedMembers);
        Assert.Equal(0, repository.SaveCount);
    }

    private static InviteOrganizationMembersCommandHandler CreateInviteHandler(
        FakeOrganizationRepository repository,
        RecordingOutbox outbox,
        IInvitationTokenService tokenService)
    {
        return new InviteOrganizationMembersCommandHandler(
            NullLogger<InviteOrganizationMembersCommandHandler>.Instance,
            repository,
            outbox,
            tokenService,
            new AllowWorkspaceAccess(),
            Options.Create(new InvitationOptions
            {
                ClientBaseUrl = "https://client.taskflow.test/",
                LifetimeHours = 24
            }),
            new FixedTimeProvider(Now));
    }

    private static AcceptOrganizationInvitationCommandHandler CreateAcceptHandler(
        FakeOrganizationRepository repository,
        IInvitationTokenService tokenService,
        string userId,
        string email)
    {
        return new AcceptOrganizationInvitationCommandHandler(
            repository,
            tokenService,
            new FixedUserContext(new CurrentUser(userId, email, [])),
            new FixedTimeProvider(Now));
    }

    private static OrganizationInvitation Invitation(DateTimeOffset expiresAt)
    {
        var organization = new Organization
        {
            Id = 7,
            Name = "Platform",
            Members = []
        };
        return new OrganizationInvitation
        {
            OrganizationId = organization.Id,
            Organization = organization,
            InvitedEmail = "MEMBER@EXAMPLE.COM",
            TokenHash = new string('A', 64),
            CreatedAtUtc = Now.UtcDateTime,
            ExpiresAtUtc = expiresAt.UtcDateTime
        };
    }

    private static string ExtractToken(string htmlBody)
    {
        const string marker = "token=";
        var start = htmlBody.IndexOf(marker, StringComparison.Ordinal) + marker.Length;
        var end = htmlBody.IndexOf('\'', start);
        return htmlBody[start..end];
    }

    private sealed class FakeOrganizationRepository : IOrganizationRepository
    {
        public Organization? Organization { get; init; }
        public OrganizationInvitation? OutstandingInvitation { get; init; }
        public OrganizationInvitation? InvitationByHash { get; init; }
        public List<OrganizationInvitation> AddedInvitations { get; } = [];
        public List<OrganizationMember> AddedMembers { get; } = [];
        public int SaveCount { get; private set; }

        public Task<int> CreateOrganization(Organization organization) => throw new NotSupportedException();
        public Task<IEnumerable<Organization>> GetAllForUserAsync(string userId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Organization?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Organization);
        public Task<OrganizationRole?> GetMemberRoleAsync(int organizationId, string userId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task AddMemberAsync(OrganizationMember member, CancellationToken cancellationToken = default)
        {
            AddedMembers.Add(member);
            return Task.CompletedTask;
        }

        public Task AddInvitationAsync(OrganizationInvitation invitation, CancellationToken cancellationToken = default)
        {
            AddedInvitations.Add(invitation);
            return Task.CompletedTask;
        }

        public Task<OrganizationInvitation?> GetOutstandingInvitationAsync(
            int organizationId,
            string normalizedEmail,
            CancellationToken cancellationToken = default) => Task.FromResult(OutstandingInvitation);

        public Task<OrganizationInvitation?> GetInvitationByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default) => Task.FromResult(InvitationByHash);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingOutbox : IEmailOutboxRepository
    {
        public List<(string Recipient, string Subject, string HtmlBody)> Messages { get; } = [];

        public Task EnqueueAsync(
            string recipient,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default)
        {
            Messages.Add((recipient, subject, htmlBody));
            return Task.CompletedTask;
        }
    }

    private sealed class FixedUserContext(CurrentUser user) : IUserContext
    {
        public CurrentUser? GetCurrentUser() => user;
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class AllowWorkspaceAccess : IWorkspaceAccessService
    {
        public CurrentUser GetCurrentUser() => throw new NotSupportedException();
        public Task<OrganizationRole> RequireOrganizationMemberAsync(int organizationId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task RequireOrganizationManagementAsync(int organizationId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RequireProjectCreationAsync(int organizationId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Project> RequireProjectViewAsync(int projectId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Project> RequireProjectContributionAsync(int projectId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Project> RequireProjectManagementAsync(int projectId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task EnsureOrganizationMemberAsync(int organizationId, string userId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task EnsureProjectMemberAsync(Project project, string userId) => throw new NotSupportedException();
    }
}
