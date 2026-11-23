namespace TaskFlow.Application.Orgs.Invitations;

public interface IInvitationTokenService
{
    InvitationToken CreateToken();
    string HashToken(string rawToken);
}

public readonly record struct InvitationToken(string RawToken, string Hash);
