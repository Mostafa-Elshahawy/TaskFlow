using TaskFlow.Application.Common;
using Xunit;

namespace TaskFlow.Tests.Application;

public class ConcurrencyTokenTests
{
    [Fact]
    public void Encode_UsesBase64ForEightByteRowVersion()
    {
        var encoded = ConcurrencyToken.Encode([1, 2, 3, 4, 5, 6, 7, 8]);

        Assert.Equal("AQIDBAUGBwg=", encoded);
    }

    [Fact]
    public void Decode_ReturnsOriginalEightBytes()
    {
        var decoded = ConcurrencyToken.Decode("AQIDBAUGBwg=");

        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, decoded);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-base64")]
    [InlineData("AQIDBA==")]
    public void Decode_RejectsMalformedOrNonRowVersionTokens(string token)
    {
        Assert.Throws<ArgumentException>(() => ConcurrencyToken.Decode(token));
    }
}
