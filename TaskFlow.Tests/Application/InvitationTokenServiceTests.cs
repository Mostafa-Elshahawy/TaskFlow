using TaskFlow.Application.Orgs.Invitations;
using Xunit;

namespace TaskFlow.Tests.Application;

public class InvitationTokenServiceTests
{
    [Fact]
    public void CreateToken_ReturnsDistinctRawTokensWithMatchingHashes()
    {
        var service = new InvitationTokenService();

        var first = service.CreateToken();
        var second = service.CreateToken();

        Assert.NotEqual(first.RawToken, second.RawToken);
        Assert.NotEqual(first.Hash, second.Hash);
        Assert.Equal(service.HashToken(first.RawToken), first.Hash);
        Assert.Equal(service.HashToken(second.RawToken), second.Hash);
        Assert.NotEqual(first.RawToken, first.Hash);
    }

    [Fact]
    public void HashToken_UsesSha256WithStableUppercaseHexEncoding()
    {
        var service = new InvitationTokenService();

        var hash = service.HashToken("test-token");

        Assert.Equal("4C5DC9B7708905F77F5E5D16316B5DFB425E68CB326DCD55A860E90A7707031E", hash);
    }
}
