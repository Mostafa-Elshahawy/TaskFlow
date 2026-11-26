using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Tasks.Commands.AssignTask;
using TaskFlow.Application.Tasks.Commands.DeleteTask;
using TaskFlow.Application.Tasks.Commands.UpdateTask;
using TaskFlow.Application.Tasks.Queries.GetTaskHistory;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using Xunit;
using TaskStatus = TaskFlow.Domain.Entites.TaskStatus;

namespace TaskFlow.Tests.Application;

public class TaskWorkflowHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 26, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Update_StatusChange_AppendsHistoryAndForwardsExpectedVersion()
    {
        var task = CreateTask(TaskStatus.Open);
        var repository = new FakeTaskRepository(task);
        var access = new FakeWorkspaceAccessService();
        var handler = CreateUpdateHandler(repository, access);

        await handler.Handle(CreateUpdateCommand(TaskStatus.InProgress), CancellationToken.None);

        var history = Assert.Single(task.StatusHistory);
        Assert.Equal(TaskStatus.Open, history.FromStatus);
        Assert.Equal(TaskStatus.InProgress, history.ToStatus);
        Assert.Equal("user-1", history.ChangedByUserId);
        Assert.Equal(Now, history.ChangedAtUtc);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, repository.ExpectedRowVersion);
        Assert.Equal(Now.UtcDateTime, task.UpdatedAt);
    }

    [Fact]
    public async Task Update_WithoutStatusChange_DoesNotAppendHistory()
    {
        var task = CreateTask(TaskStatus.Open);
        var repository = new FakeTaskRepository(task);
        var handler = CreateUpdateHandler(repository, new FakeWorkspaceAccessService());

        await handler.Handle(CreateUpdateCommand(TaskStatus.Open), CancellationToken.None);

        Assert.Empty(task.StatusHistory);
    }

    [Fact]
    public async Task Update_CompletingTask_SetsCompletedAt()
    {
        var task = CreateTask(TaskStatus.InProgress);
        var handler = CreateUpdateHandler(new FakeTaskRepository(task), new FakeWorkspaceAccessService());

        await handler.Handle(CreateUpdateCommand(TaskStatus.Completed), CancellationToken.None);

        Assert.Equal(Now.UtcDateTime, task.CompletedAt);
    }

    [Fact]
    public async Task Update_ReopeningTask_ClearsCompletedAt()
    {
        var task = CreateTask(TaskStatus.Completed);
        task.CompletedAt = Now.AddDays(-1).UtcDateTime;
        var handler = CreateUpdateHandler(new FakeTaskRepository(task), new FakeWorkspaceAccessService());

        await handler.Handle(CreateUpdateCommand(TaskStatus.InProgress), CancellationToken.None);

        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public async Task Update_WithMalformedToken_RejectsBeforeSaving()
    {
        var repository = new FakeTaskRepository(CreateTask(TaskStatus.Open));
        var handler = CreateUpdateHandler(repository, new FakeWorkspaceAccessService());
        var command = CreateUpdateCommand(TaskStatus.InProgress);
        command.RowVersion = "invalid";

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task Update_WhenRepositoryDetectsStaleVersion_ReturnsConflict()
    {
        var repository = new FakeTaskRepository(CreateTask(TaskStatus.Open))
        {
            SaveException = new ConflictException("Task was changed by another request.")
        };
        var handler = CreateUpdateHandler(repository, new FakeWorkspaceAccessService());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(CreateUpdateCommand(TaskStatus.InProgress), CancellationToken.None));
    }

    [Fact]
    public async Task Update_WhenTaskWasSoftDeleted_ReturnsConflictWithoutSaving()
    {
        var task = CreateTask(TaskStatus.Open);
        task.isDeleted = true;
        var repository = new FakeTaskRepository(task);
        var handler = CreateUpdateHandler(repository, new FakeWorkspaceAccessService());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(CreateUpdateCommand(TaskStatus.InProgress), CancellationToken.None));

        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task Delete_ForwardsExpectedVersionAndSoftDeletesTask()
    {
        var task = CreateTask(TaskStatus.Open);
        var repository = new FakeTaskRepository(task);
        var handler = new DeleteTaskCommandHandler(
            NullLogger<DeleteTaskCommandHandler>.Instance,
            repository,
            new FakeWorkspaceAccessService(),
            new FixedTimeProvider(Now));

        await handler.Handle(new DeleteTaskCommand(task.Id, "AQIDBAUGBwg="), CancellationToken.None);

        Assert.True(task.isDeleted);
        Assert.Equal(Now.UtcDateTime, task.UpdatedAt);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, repository.ExpectedRowVersion);
    }

    [Fact]
    public async Task Assign_ForwardsExpectedVersionAndUpdatesAssignee()
    {
        var task = CreateTask(TaskStatus.Open);
        var repository = new FakeTaskRepository(task);
        var assignee = new ApplicationUser { Id = "assignee-1", UserName = "assignee@example.com" };
        using var userManager = CreateUserManager(assignee);
        var handler = new AssignTaskCommandHandler(
            NullLogger<AssignTaskCommandHandler>.Instance,
            repository,
            userManager,
            new FakeWorkspaceAccessService(),
            new FixedTimeProvider(Now));

        await handler.Handle(new AssignTaskCommand
        {
            TaskId = task.Id,
            AssigneeId = assignee.Id,
            RowVersion = "AQIDBAUGBwg="
        }, CancellationToken.None);

        Assert.Equal(assignee.Id, task.AssigneeId);
        Assert.Equal(Now.UtcDateTime, task.UpdatedAt);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, repository.ExpectedRowVersion);
    }

    [Fact]
    public async Task HistoryQuery_RequiresProjectViewAndMapsEntries()
    {
        var task = CreateTask(TaskStatus.Completed);
        var repository = new FakeTaskRepository(task)
        {
            History =
            [
                new TaskStatusHistory
                {
                    Id = 11,
                    TaskId = task.Id,
                    FromStatus = TaskStatus.InProgress,
                    ToStatus = TaskStatus.Completed,
                    ChangedByUserId = "user-1",
                    ChangedBy = new ApplicationUser { UserName = "member@example.com" },
                    ChangedAtUtc = Now
                }
            ]
        };
        var access = new FakeWorkspaceAccessService();
        var handler = new GetTaskHistoryQueryHandler(repository, access);

        var result = await handler.Handle(new GetTaskHistoryQuery(task.Id), CancellationToken.None);

        var item = Assert.Single(result);
        Assert.Equal(11, item.Id);
        Assert.Equal(TaskStatus.InProgress, item.FromStatus);
        Assert.Equal(TaskStatus.Completed, item.ToStatus);
        Assert.Equal("user-1", item.ChangedByUserId);
        Assert.Equal("member@example.com", item.ChangedByName);
        Assert.Equal(Now, item.ChangedAtUtc);
        Assert.Equal(task.ProjectId, access.LastViewedProjectId);
    }

    private static UpdateTaskCommandHanlder CreateUpdateHandler(
        ITaskRepository repository,
        IWorkspaceAccessService accessService)
    {
        return new UpdateTaskCommandHanlder(
            NullLogger<UpdateTaskCommandHanlder>.Instance,
            repository,
            accessService,
            new FixedTimeProvider(Now));
    }

    private static UpdateTaskCommand CreateUpdateCommand(TaskStatus status) => new()
    {
        Id = 10,
        Title = "Updated task",
        Description = "Updated description",
        Status = status,
        Priority = TaskPriority.High,
        DueDate = Now.AddDays(2).UtcDateTime,
        RowVersion = "AQIDBAUGBwg="
    };

    private static UserManager<ApplicationUser> CreateUserManager(ApplicationUser user)
    {
        return new UserManager<ApplicationUser>(
            new FakeUserStore(user),
            Options.Create(new IdentityOptions()),
            new PasswordHasher<ApplicationUser>(),
            [],
            [],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new EmptyServiceProvider(),
            NullLogger<UserManager<ApplicationUser>>.Instance);
    }

    private static TaskEntity CreateTask(TaskStatus status) => new()
    {
        Id = 10,
        ProjectId = 20,
        Project = new Project
        {
            Id = 20,
            OrganizationId = 30,
            CreatedByUserId = "creator-1"
        },
        Title = "Task",
        Description = "Description",
        Status = status,
        Priority = TaskPriority.Low,
        CreatedByUserId = "creator-1",
        DueDate = Now.AddDays(1).UtcDateTime,
        CreatedAt = Now.AddDays(-2).UtcDateTime,
        RowVersion = [9, 9, 9, 9, 9, 9, 9, 9]
    };

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class FakeUserStore(ApplicationUser user) : IUserStore<ApplicationUser>
    {
        public void Dispose() { }
        public Task<string> GetUserIdAsync(ApplicationUser candidate, CancellationToken cancellationToken) =>
            Task.FromResult(candidate.Id);
        public Task<string?> GetUserNameAsync(ApplicationUser candidate, CancellationToken cancellationToken) =>
            Task.FromResult(candidate.UserName);
        public Task SetUserNameAsync(ApplicationUser candidate, string? userName, CancellationToken cancellationToken)
        {
            candidate.UserName = userName;
            return Task.CompletedTask;
        }
        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser candidate, CancellationToken cancellationToken) =>
            Task.FromResult(candidate.NormalizedUserName);
        public Task SetNormalizedUserNameAsync(
            ApplicationUser candidate,
            string? normalizedName,
            CancellationToken cancellationToken)
        {
            candidate.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }
        public Task<IdentityResult> CreateAsync(ApplicationUser candidate, CancellationToken cancellationToken) =>
            Task.FromResult(IdentityResult.Success);
        public Task<IdentityResult> UpdateAsync(ApplicationUser candidate, CancellationToken cancellationToken) =>
            Task.FromResult(IdentityResult.Success);
        public Task<IdentityResult> DeleteAsync(ApplicationUser candidate, CancellationToken cancellationToken) =>
            Task.FromResult(IdentityResult.Success);
        public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
            Task.FromResult<ApplicationUser?>(user.Id == userId ? user : null);
        public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
            Task.FromResult<ApplicationUser?>(user.NormalizedUserName == normalizedUserName ? user : null);
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }

    private sealed class FakeTaskRepository(TaskEntity task) : ITaskRepository
    {
        public byte[]? ExpectedRowVersion { get; private set; }
        public int SaveCount { get; private set; }
        public Exception? SaveException { get; init; }
        public IReadOnlyList<TaskStatusHistory> History { get; init; } = [];

        public Task<int> Create(TaskEntity entity) => Task.FromResult(entity.Id);
        public Task<TaskEntity?> GetById(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult<TaskEntity?>(id == task.Id ? task : null);

        public Task<TaskEntity?> GetByIdForMutation(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult<TaskEntity?>(id == task.Id ? task : null);

        public Task<IEnumerable<TaskEntity>> GetFilteredTasksForUser(
            TaskFilter taskFilter,
            string userId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<TaskEntity>>([task]);

        public Task SaveChangesAsync(
            TaskEntity entity,
            byte[] expectedRowVersion,
            CancellationToken cancellationToken = default)
        {
            SaveCount++;
            ExpectedRowVersion = expectedRowVersion;
            return SaveException is null ? Task.CompletedTask : Task.FromException(SaveException);
        }

        public Task DeleteAsync(
            TaskEntity entity,
            byte[] expectedRowVersion,
            CancellationToken cancellationToken = default) =>
            SaveChangesAsync(entity, expectedRowVersion, cancellationToken);

        public Task<IReadOnlyList<TaskStatusHistory>> GetStatusHistoryAsync(
            int taskId,
            CancellationToken cancellationToken = default) => Task.FromResult(History);
    }

    private sealed class FakeWorkspaceAccessService : IWorkspaceAccessService
    {
        public int? LastViewedProjectId { get; private set; }

        public CurrentUser GetCurrentUser() => new("user-1", "member@example.com", [UserRoles.User]);

        public Task<Project> RequireProjectViewAsync(int projectId, CancellationToken cancellationToken = default)
        {
            LastViewedProjectId = projectId;
            return Task.FromResult(new Project { Id = projectId });
        }

        public Task<Project> RequireProjectContributionAsync(int projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new Project { Id = projectId });

        public Task<Project> RequireProjectManagementAsync(int projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new Project { Id = projectId });

        public Task<OrganizationRole> RequireOrganizationMemberAsync(int organizationId, CancellationToken cancellationToken = default) =>
            Task.FromResult(OrganizationRole.Member);

        public Task RequireOrganizationManagementAsync(int organizationId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RequireProjectCreationAsync(int organizationId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task EnsureOrganizationMemberAsync(int organizationId, string userId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task EnsureProjectMemberAsync(Project project, string userId) => Task.CompletedTask;
    }
}
