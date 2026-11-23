namespace TaskFlow.Domain.Repositories;

public interface IEmailOutboxRepository
{
    Task EnqueueAsync(
        string recipient,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
