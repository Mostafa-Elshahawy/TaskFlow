namespace TaskFlow.Infrastructure.Seeders;

public interface IIdentitySeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
