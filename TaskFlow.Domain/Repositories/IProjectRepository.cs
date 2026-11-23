using TaskFlow.Domain.Entites;

namespace TaskFlow.Domain.Repositories;

public interface IProjectRepository
{
    Task<int> CreateProjectAsync(Project project);
    Task<Project?> GetProjectByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetAllProjectsForUserAsync(
        string userId,
        int? organizationId = null,
        string? createdByUserId = null,
        CancellationToken cancellationToken = default);
    Task DeleteProjectAsync(int projectId, CancellationToken cancellationToken = default);
    Task AddMemberAsync(int projectId, string userId, CancellationToken cancellationToken = default);
    Task UpdateDetailsAsync(
        int projectId,
        string name,
        string description,
        CancellationToken cancellationToken = default);
}
