using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskFlow.Infrastructure.BackgroundJobs;

internal sealed class EmailOutboxWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<EmailOutboxWorker> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var processor = scope.ServiceProvider.GetRequiredService<EmailOutboxProcessor>();
                await processor.ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Email outbox processing cycle failed.");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }
}
