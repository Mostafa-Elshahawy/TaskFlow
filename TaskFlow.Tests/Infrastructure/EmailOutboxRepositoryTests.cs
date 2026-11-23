using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.BackgroundJobs;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Repositories;
using Xunit;

namespace TaskFlow.Tests.Infrastructure;

public class EmailOutboxRepositoryTests
{
    [Fact]
    public async Task EnqueueAsync_ProtectsRawInvitationTokenBeforePersistence()
    {
        var keysPath = Path.Combine(Path.GetTempPath(), $"taskflow-keys-{Guid.NewGuid():N}");
        Directory.CreateDirectory(keysPath);

        try
        {
            var dataProtection = DataProtectionProvider.Create(new DirectoryInfo(keysPath));
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using var dbContext = new ApplicationDBContext(options);
            var repository = new EmailOutboxRepository(dbContext, dataProtection, TimeProvider.System);
            const string htmlBody = "<a href='http://localhost:4200/accept-invite?token=raw-secret-token'>Accept</a>";

            await repository.EnqueueAsync(
                "member@taskflow.test",
                "Organization invitation",
                htmlBody,
                CancellationToken.None);
            await dbContext.SaveChangesAsync();

            var stored = await dbContext.Set<EmailOutboxMessage>().SingleAsync();
            Assert.DoesNotContain("raw-secret-token", stored.ProtectedHtmlBody);

            var protector = dataProtection.CreateProtector("TaskFlow.EmailOutbox.v1");
            Assert.Equal(htmlBody, protector.Unprotect(stored.ProtectedHtmlBody));
        }
        finally
        {
            Directory.Delete(keysPath, recursive: true);
        }
    }

    [Fact]
    public async Task ProcessBatchAsync_DeliversUnprotectedBodyAndMarksMessageProcessed()
    {
        var fixture = await OutboxFixture.CreateAsync();
        await using var cleanup = fixture;
        var sender = new RecordingEmailSender();
        var processor = fixture.CreateProcessor(sender);

        var processed = await processor.ProcessBatchAsync(CancellationToken.None);

        var message = await fixture.DbContext.Set<EmailOutboxMessage>().SingleAsync();
        Assert.Equal(1, processed);
        Assert.NotNull(message.ProcessedAtUtc);
        Assert.Equal("<p>token=raw-secret-token</p>", Assert.Single(sender.Bodies));
    }

    [Fact]
    public async Task ProcessBatchAsync_WhenDeliveryFails_RecordsRetryWithoutProcessingMessage()
    {
        var fixture = await OutboxFixture.CreateAsync();
        await using var cleanup = fixture;
        var processor = fixture.CreateProcessor(new ThrowingEmailSender());

        var processed = await processor.ProcessBatchAsync(CancellationToken.None);

        var message = await fixture.DbContext.Set<EmailOutboxMessage>().SingleAsync();
        Assert.Equal(0, processed);
        Assert.Null(message.ProcessedAtUtc);
        Assert.Equal(1, message.AttemptCount);
        Assert.True(message.NextAttemptAtUtc > fixture.UtcNow);
        Assert.NotNull(message.LastError);
    }

    private sealed class OutboxFixture : IAsyncDisposable
    {
        private readonly string _keysPath;
        private readonly IDataProtectionProvider _dataProtection;

        private OutboxFixture(
            string keysPath,
            IDataProtectionProvider dataProtection,
            ApplicationDBContext dbContext,
            DateTimeOffset utcNow)
        {
            _keysPath = keysPath;
            _dataProtection = dataProtection;
            DbContext = dbContext;
            UtcNow = utcNow;
        }

        public ApplicationDBContext DbContext { get; }
        public DateTimeOffset UtcNow { get; }

        public static async Task<OutboxFixture> CreateAsync()
        {
            var keysPath = Path.Combine(Path.GetTempPath(), $"taskflow-keys-{Guid.NewGuid():N}");
            Directory.CreateDirectory(keysPath);
            var dataProtection = DataProtectionProvider.Create(new DirectoryInfo(keysPath));
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new ApplicationDBContext(options);
            var utcNow = new DateTimeOffset(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);
            var repository = new EmailOutboxRepository(
                dbContext,
                dataProtection,
                new FixedTimeProvider(utcNow));
            await repository.EnqueueAsync(
                "member@taskflow.test",
                "Organization invitation",
                "<p>token=raw-secret-token</p>",
                CancellationToken.None);
            await dbContext.SaveChangesAsync();
            return new OutboxFixture(
                keysPath,
                dataProtection,
                dbContext,
                utcNow);
        }

        public EmailOutboxProcessor CreateProcessor(IEmailSender sender)
        {
            return new EmailOutboxProcessor(
                DbContext,
                sender,
                _dataProtection,
                new FixedTimeProvider(UtcNow),
                NullLogger<EmailOutboxProcessor>.Instance);
        }

        public async ValueTask DisposeAsync()
        {
            await DbContext.DisposeAsync();
            Directory.Delete(_keysPath, recursive: true);
        }
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class RecordingEmailSender : IEmailSender
    {
        public List<string> Bodies { get; } = [];

        public Task SendEmailAsync(string to, string subject, string body)
        {
            Bodies.Add(body);
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            throw new InvalidOperationException("SMTP unavailable");
        }
    }
}
