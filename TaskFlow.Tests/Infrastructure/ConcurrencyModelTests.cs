using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entites;
using TaskFlow.Infrastructure.Persistence;
using Xunit;

namespace TaskFlow.Tests.Infrastructure;

public class ConcurrencyModelTests
{
    [Fact]
    public void TaskAndProjectRowVersions_AreConfiguredAsConcurrencyTokens()
    {
        using var context = CreateContext();

        var taskProperty = context.Model.FindEntityType(typeof(TaskEntity))!
            .FindProperty(nameof(TaskEntity.RowVersion))!;
        var projectProperty = context.Model.FindEntityType(typeof(Project))!
            .FindProperty(nameof(Project.RowVersion))!;

        Assert.True(taskProperty.IsConcurrencyToken);
        Assert.True(projectProperty.IsConcurrencyToken);
        Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate, taskProperty.ValueGenerated);
        Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate, projectProperty.ValueGenerated);
    }

    [Fact]
    public void TaskStatusHistory_HasRequiredTaskAndUserRelationships()
    {
        using var context = CreateContext();
        var historyType = context.Model.FindEntityType(typeof(TaskStatusHistory))!;

        Assert.NotNull(historyType.FindProperty(nameof(TaskStatusHistory.TaskId)));
        Assert.NotNull(historyType.FindProperty(nameof(TaskStatusHistory.ChangedByUserId)));
        Assert.Contains(historyType.GetForeignKeys(), key => key.PrincipalEntityType.ClrType == typeof(TaskEntity));
        Assert.Contains(historyType.GetForeignKeys(), key => key.PrincipalEntityType.ClrType == typeof(ApplicationUser));
    }

    private static ApplicationDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDBContext(options);
    }
}
