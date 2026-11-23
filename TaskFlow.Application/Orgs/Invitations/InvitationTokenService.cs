using System.Security.Cryptography;
using System.Text;

namespace TaskFlow.Application.Orgs.Invitations;

public sealed class InvitationTokenService : IInvitationTokenService
{
    public InvitationToken CreateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var rawToken = Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        return new InvitationToken(rawToken, HashToken(rawToken));
    }

    public string HashToken(string rawToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawToken);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
    }
}
