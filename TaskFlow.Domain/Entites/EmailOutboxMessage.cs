namespace TaskFlow.Domain.Entites;

public sealed class EmailOutboxMessage
{
    public long Id { get; set; }
    public string Recipient { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string ProtectedHtmlBody { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime NextAttemptAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public int AttemptCount { get; set; }
    public string? LastError { get; set; }
}
