namespace TaskFlow.Application.Common;

public static class ConcurrencyToken
{
    private const int RowVersionLength = 8;

    public static string Encode(byte[] rowVersion)
    {
        if (rowVersion.Length != RowVersionLength)
            throw new ArgumentException("Concurrency token must contain exactly eight bytes.", nameof(rowVersion));

        return Convert.ToBase64String(rowVersion);
    }

    public static byte[] Decode(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("A concurrency token is required.", nameof(token));

        try
        {
            var rowVersion = Convert.FromBase64String(token);
            if (rowVersion.Length != RowVersionLength)
                throw new ArgumentException("Concurrency token must contain exactly eight bytes.", nameof(token));

            return rowVersion;
        }
        catch (FormatException exception)
        {
            throw new ArgumentException("Concurrency token is not valid base64.", nameof(token), exception);
        }
    }
}
