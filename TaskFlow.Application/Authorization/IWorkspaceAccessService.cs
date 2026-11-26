using TaskFlow.Application.Users;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Authorization;

public interface IWorkspaceAccessService
{
    CurrentUser GetCurrentUser();
    Task<OrganizationRole> RequireOrganizationMemberAsync(int organizationId, CancellationToken cancellationToken = default);
    Task RequireOrganizationManagementAsync(int organizationId, CancellationToken cancellationToken = default);
    Task RequireProjectCreationAsync(int organizationId, CancellationToken cancellationToken = default);
    Task<Project> RequireProjectViewAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Project> RequireProjectContributionAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Project> RequireProjectContributionAsync(Project project, CancellationToken cancellationToken = default) =>
        RequireProjectContributionAsync(project.Id, cancellationToken);
    Task<Project> RequireProjectManagementAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Project> RequireProjectManagementAsync(Project project, CancellationToken cancellationToken = default) =>
        RequireProjectManagementAsync(project.Id, cancellationToken);
    Task EnsureOrganizationMemberAsync(int organizationId, string userId, CancellationToken cancellationToken = default);
    Task EnsureProjectMemberAsync(Project project, string userId);
}
