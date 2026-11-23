using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.BackgroundJobs;

public sealed class EmailOutboxProcessor(
    ApplicationDBContext dbContext,
    IEmailSender emailSender,
    IDataProtectionProvider dataProtectionProvider,
    TimeProvider timeProvider,
    ILogger<EmailOutboxProcessor> logger)
{
    private const string ProtectionPurpose = "TaskFlow.EmailOutbox.v1";
    private const int MaximumAttempts = 5;
    private const int BatchSize = 10;

    public async Task<int> ProcessBatchAsync(CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var messages = await dbContext.Set<EmailOutboxMessage>()
            .Where(message =>
                message.ProcessedAtUtc == null &&
                message.AttemptCount < MaximumAttempts &&
                message.NextAttemptAtUtc <= now)
            .OrderBy(message => message.CreatedAtUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        var protector = dataProtectionProvider.CreateProtector(ProtectionPurpose);
        var processed = 0;

        foreach (var message in messages)
        {
            try
            {
                var body = protector.Unprotect(message.ProtectedHtmlBody);
                await emailSender.SendEmailAsync(message.Recipient, message.Subject, body);
                message.ProcessedAtUtc = now;
                message.LastError = null;
                processed++;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                message.AttemptCount++;
                var delayMinutes = Math.Min(Math.Pow(2, message.AttemptCount), 60);
                message.NextAttemptAtUtc = now.AddMinutes(delayMinutes);
                message.LastError = exception.Message.Length <= 2000
                    ? exception.Message
                    : exception.Message[..2000];

                logger.LogWarning(
                    exception,
                    "Email outbox delivery failed for MessageId {MessageId}; attempt {AttemptCount}.",
                    message.Id,
                    message.AttemptCount);
            }
        }

        if (messages.Count > 0)
            await dbContext.SaveChangesAsync(cancellationToken);

        return processed;
    }
}
