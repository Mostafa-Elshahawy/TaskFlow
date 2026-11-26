using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Extensions;
using TaskFlow.Infrastructure.Persistence;
using Testcontainers.MsSql;
using Xunit;
using TaskStatus = TaskFlow.Domain.Entites.TaskStatus;

namespace TaskFlow.Tests.Infrastructure;

public sealed class SqlServerConcurrencyIntegrationTests
{
    [SqlServerIntegrationFact]
    public async Task StaleMutations_UseRowVersionsAndRollbackStatusHistory()
    {
        await using var container = new MsSqlBuilder(
            "mcr.microsoft.com/mssql/server:2022-latest").Build();
        await container.StartAsync();
        var connectionString = container.GetConnectionString();
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseSqlServer(connectionString)
            .Options;
        await using (var context = new ApplicationDBContext(options))
            await context.Database.MigrateAsync();

        await AssertTaskConflictAsync(connectionString);
        await AssertProjectConflictAsync(connectionString, delete: false);
        await AssertProjectConflictAsync(connectionString, delete: true);
    }

    private static async Task AssertTaskConflictAsync(string connectionString)
    {
        await using var services = BuildServices(connectionString);
        var seeded = await SeedWorkspaceAsync(services);
        await using var firstScope = services.CreateAsyncScope();
        await using var secondScope = services.CreateAsyncScope();
        var repository = firstScope.ServiceProvider.GetRequiredService<ITaskRepository>();
        var firstTask = await repository.GetByIdForMutation(seeded.TaskId)
            ?? throw new InvalidOperationException("Seeded task was not found.");
        var expectedRowVersion = firstTask.RowVersion.ToArray();
        var secondContext = secondScope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var secondTask = await secondContext.Set<TaskEntity>().SingleAsync(task => task.Id == seeded.TaskId);

        secondTask.Title = "Concurrent title";
        await secondContext.SaveChangesAsync();

        firstTask.Status = TaskStatus.InProgress;
        firstTask.StatusHistory.Add(new TaskStatusHistory
        {
            TaskId = firstTask.Id,
            FromStatus = TaskStatus.Open,
            ToStatus = TaskStatus.InProgress,
            ChangedByUserId = seeded.UserId,
            ChangedAtUtc = DateTimeOffset.UtcNow
        });

        await Assert.ThrowsAsync<ConflictException>(() =>
            repository.SaveChangesAsync(firstTask, expectedRowVersion));

        await using var verificationScope = services.CreateAsyncScope();
        var verificationContext = verificationScope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var persistedTask = await verificationContext.Set<TaskEntity>()
            .SingleAsync(task => task.Id == seeded.TaskId);
        Assert.Equal(TaskStatus.Open, persistedTask.Status);
        Assert.False(await verificationContext.Set<TaskStatusHistory>()
            .AnyAsync(history => history.TaskId == seeded.TaskId));
    }

    private static async Task AssertProjectConflictAsync(string connectionString, bool delete)
    {
        await using var services = BuildServices(connectionString);
        var seeded = await SeedWorkspaceAsync(services);
        await using var firstScope = services.CreateAsyncScope();
        await using var secondScope = services.CreateAsyncScope();
        var repository = firstScope.ServiceProvider.GetRequiredService<IProjectRepository>();
        var firstProject = await repository.GetProjectByIdForMutationAsync(seeded.ProjectId)
            ?? throw new InvalidOperationException("Seeded project was not found.");
        var expectedRowVersion = firstProject.RowVersion.ToArray();
        var secondContext = secondScope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var secondProject = await secondContext.Set<Project>()
            .SingleAsync(project => project.Id == seeded.ProjectId);

        secondProject.Description = "Concurrent description";
        await secondContext.SaveChangesAsync();

        if (delete)
        {
            await Assert.ThrowsAsync<ConflictException>(() =>
                repository.DeleteProjectAsync(seeded.ProjectId, expectedRowVersion));
        }
        else
        {
            await Assert.ThrowsAsync<ConflictException>(() =>
                repository.UpdateDetailsAsync(
                    seeded.ProjectId,
                    "Stale name",
                    "Stale description",
                    expectedRowVersion));
        }
    }

    private static ServiceProvider BuildServices(string connectionString)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TaskFlowDB"] = connectionString
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(configuration);
        return services.BuildServiceProvider();
    }

    private static async Task<SeededWorkspace> SeedWorkspaceAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var suffix = Guid.NewGuid().ToString("N");
        var user = new ApplicationUser
        {
            Id = $"user-{suffix}",
            UserName = $"user-{suffix}@example.com",
            NormalizedUserName = $"USER-{suffix}@EXAMPLE.COM",
            Email = $"user-{suffix}@example.com",
            NormalizedEmail = $"USER-{suffix}@EXAMPLE.COM",
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        var organization = new Organization
        {
            Name = $"Organization {suffix}",
            Description = "Integration test organization",
            OwnerId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        var project = new Project
        {
            Name = $"Project {suffix}",
            Description = "Integration test project",
            CreatedByUserId = user.Id,
            Organization = organization,
            CreatedAt = DateTime.UtcNow
        };
        var task = new TaskEntity
        {
            Title = $"Task {suffix}",
            Description = "Integration test task",
            Status = TaskStatus.Open,
            Priority = TaskPriority.Low,
            Project = project,
            CreatedByUserId = user.Id,
            DueDate = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow
        };

        context.AddRange(user, organization, project, task);
        await context.SaveChangesAsync();
        return new SeededWorkspace(user.Id, project.Id, task.Id);
    }

    private sealed record SeededWorkspace(string UserId, int ProjectId, int TaskId);
}

public sealed class SqlServerIntegrationFactAttribute : FactAttribute
{
    public SqlServerIntegrationFactAttribute()
    {
        if (!string.Equals(
                Environment.GetEnvironmentVariable("TASKFLOW_RUN_SQL_INTEGRATION_TESTS"),
                "1",
                StringComparison.Ordinal))
        {
            Skip = "Set TASKFLOW_RUN_SQL_INTEGRATION_TESTS=1 to run Docker-backed SQL Server tests.";
        }
    }
}
