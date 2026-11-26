using TaskFlow.Application.Users;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Authorization;

public class WorkspaceAccessService(
    IUserContext userContext,
    IOrganizationRepository organizationRepository,
    IProjectRepository projectRepository) : IWorkspaceAccessService
{
    public CurrentUser GetCurrentUser()
    {
        return userContext.GetCurrentUser()
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }

    public async Task<OrganizationRole> RequireOrganizationMemberAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        var currentUser = GetCurrentUser();
        var role = await organizationRepository.GetMemberRoleAsync(
            organizationId,
            currentUser.Id,
            cancellationToken);

        return role ?? throw new ForbidException();
    }

    public async Task RequireOrganizationManagementAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        var role = await RequireOrganizationMemberAsync(organizationId, cancellationToken);
        if (role is not OrganizationRole.Owner and not OrganizationRole.Admin)
            throw new ForbidException();
    }

    public async Task RequireProjectCreationAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        var role = await RequireOrganizationMemberAsync(organizationId, cancellationToken);
        if (!CanManageProjects(role))
            throw new ForbidException();
    }

    public async Task<Project> RequireProjectViewAsync(
        int projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await GetProjectAsync(projectId, cancellationToken);
        await RequireOrganizationMemberAsync(project.OrganizationId, cancellationToken);
        return project;
    }

    public async Task<Project> RequireProjectContributionAsync(
        int projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await GetProjectAsync(projectId, cancellationToken);
        return await RequireProjectContributionAsync(project, cancellationToken);
    }

    public async Task<Project> RequireProjectContributionAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        var currentUser = GetCurrentUser();
        var role = await RequireOrganizationMemberAsync(project.OrganizationId, cancellationToken);

        var canContribute = CanManageProjects(role) ||
                            project.CreatedByUserId == currentUser.Id ||
                            project.Members.Any(member => member.UserId == currentUser.Id);

        if (!canContribute)
            throw new ForbidException();

        return project;
    }

    public async Task<Project> RequireProjectManagementAsync(
        int projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await GetProjectAsync(projectId, cancellationToken);
        return await RequireProjectManagementAsync(project, cancellationToken);
    }

    public async Task<Project> RequireProjectManagementAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        var currentUser = GetCurrentUser();
        var role = await RequireOrganizationMemberAsync(project.OrganizationId, cancellationToken);

        if (!CanManageProjects(role) && project.CreatedByUserId != currentUser.Id)
            throw new ForbidException();

        return project;
    }

    public async Task EnsureOrganizationMemberAsync(
        int organizationId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var role = await organizationRepository.GetMemberRoleAsync(organizationId, userId, cancellationToken);
        if (role is null)
            throw new ForbidException();
    }

    public Task EnsureProjectMemberAsync(Project project, string userId)
    {
        if (project.Members.All(member => member.UserId != userId))
            throw new ForbidException();

        return Task.CompletedTask;
    }

    private async Task<Project> GetProjectAsync(int projectId, CancellationToken cancellationToken)
    {
        return await projectRepository.GetProjectByIdAsync(projectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), projectId.ToString());
    }

    private static bool CanManageProjects(OrganizationRole role)
    {
        return role is OrganizationRole.Owner or OrganizationRole.Admin or OrganizationRole.Manager;
    }
}
