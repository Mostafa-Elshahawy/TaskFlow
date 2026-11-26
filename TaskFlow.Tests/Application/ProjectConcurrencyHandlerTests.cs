using Microsoft.Extensions.Logging.Abstractions;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Projects.Commands.DeleteProject;
using TaskFlow.Application.Projects.Commands.UpdateProject;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application;

public class ProjectConcurrencyHandlerTests
{
    [Fact]
    public async Task Update_ForwardsDecodedExpectedVersion()
    {
        var repository = new FakeProjectRepository();
        var handler = new UpdateProjectCommandHandler(
            NullLogger<UpdateProjectCommandHandler>.Instance,
            repository,
            new FakeWorkspaceAccessService());

        await handler.Handle(new UpdateProjectCommand
        {
            Id = 12,
            Name = "Updated",
            Description = "Updated description",
            RowVersion = "AQIDBAUGBwg="
        }, CancellationToken.None);

        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, repository.UpdateExpectedVersion);
    }

    [Fact]
    public async Task Delete_ForwardsDecodedExpectedVersion()
    {
        var repository = new FakeProjectRepository();
        var handler = new DeleteProjectCommandHandler(
            NullLogger<DeleteProjectCommandHandler>.Instance,
            repository,
            new FakeWorkspaceAccessService());

        await handler.Handle(new DeleteProjectCommand(12, "AQIDBAUGBwg="), CancellationToken.None);

        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, repository.DeleteExpectedVersion);
    }

    [Fact]
    public async Task Update_WithMalformedToken_RejectsBeforeRepositoryWrite()
    {
        var repository = new FakeProjectRepository();
        var handler = new UpdateProjectCommandHandler(
            NullLogger<UpdateProjectCommandHandler>.Instance,
            repository,
            new FakeWorkspaceAccessService());

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(new UpdateProjectCommand
        {
            Id = 12,
            Name = "Updated",
            Description = "Updated description",
            RowVersion = "invalid"
        }, CancellationToken.None));

        Assert.Null(repository.UpdateExpectedVersion);
    }

    [Fact]
    public async Task Update_WhenProjectWasSoftDeleted_ReturnsConflictWithoutWriting()
    {
        var repository = new FakeProjectRepository
        {
            Project = new Project { Id = 12, isDeleted = true }
        };
        var handler = new UpdateProjectCommandHandler(
            NullLogger<UpdateProjectCommandHandler>.Instance,
            repository,
            new FakeWorkspaceAccessService());

        await Assert.ThrowsAsync<TaskFlow.Domain.Exceptions.ConflictException>(() =>
            handler.Handle(new UpdateProjectCommand
            {
                Id = 12,
                Name = "Updated",
                Description = "Updated description",
                RowVersion = "AQIDBAUGBwg="
            }, CancellationToken.None));

        Assert.Null(repository.UpdateExpectedVersion);
    }

    private sealed class FakeProjectRepository : IProjectRepository
    {
        public Project Project { get; init; } = new() { Id = 12 };
        public byte[]? UpdateExpectedVersion { get; private set; }
        public byte[]? DeleteExpectedVersion { get; private set; }

        public Task<int> CreateProjectAsync(Project project) => Task.FromResult(project.Id);
        public Task<Project?> GetProjectByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Project?>(Project);
        public Task<Project?> GetProjectByIdForMutationAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Project?>(Project);
        public Task<IEnumerable<Project>> GetAllProjectsForUserAsync(
            string userId,
            int? organizationId = null,
            string? createdByUserId = null,
            CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Project>>([]);

        public Task DeleteProjectAsync(
            int projectId,
            byte[] expectedRowVersion,
            CancellationToken cancellationToken = default)
        {
            DeleteExpectedVersion = expectedRowVersion;
            return Task.CompletedTask;
        }

        public Task UpdateDetailsAsync(
            int projectId,
            string name,
            string description,
            byte[] expectedRowVersion,
            CancellationToken cancellationToken = default)
        {
            UpdateExpectedVersion = expectedRowVersion;
            return Task.CompletedTask;
        }

        public Task AddMemberAsync(int projectId, string userId, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeWorkspaceAccessService : IWorkspaceAccessService
    {
        public CurrentUser GetCurrentUser() => new("user-1", "member@example.com", [UserRoles.User]);
        public Task<Project> RequireProjectManagementAsync(int projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new Project { Id = projectId });
        public Task<Project> RequireProjectViewAsync(int projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new Project { Id = projectId });
        public Task<Project> RequireProjectContributionAsync(int projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new Project { Id = projectId });
        public Task<OrganizationRole> RequireOrganizationMemberAsync(int organizationId, CancellationToken cancellationToken = default) =>
            Task.FromResult(OrganizationRole.Member);
        public Task RequireOrganizationManagementAsync(int organizationId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RequireProjectCreationAsync(int organizationId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task EnsureOrganizationMemberAsync(int organizationId, string userId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task EnsureProjectMemberAsync(Project project, string userId) => Task.CompletedTask;
    }
}
