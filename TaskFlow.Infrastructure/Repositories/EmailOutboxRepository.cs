using Microsoft.AspNetCore.DataProtection;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

public sealed class EmailOutboxRepository(
    ApplicationDBContext dbContext,
    IDataProtectionProvider dataProtectionProvider,
    TimeProvider timeProvider) : IEmailOutboxRepository
{
    private const string ProtectionPurpose = "TaskFlow.EmailOutbox.v1";

    public async Task EnqueueAsync(
        string recipient,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var protector = dataProtectionProvider.CreateProtector(ProtectionPurpose);
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var message = new EmailOutboxMessage
        {
            Recipient = recipient,
            Subject = subject,
            ProtectedHtmlBody = protector.Protect(htmlBody),
            CreatedAtUtc = now,
            NextAttemptAtUtc = now
        };

        await dbContext.Set<EmailOutboxMessage>().AddAsync(message, cancellationToken);
    }
}
