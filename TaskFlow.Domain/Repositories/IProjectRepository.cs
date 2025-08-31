using TaskFlow.Domain.Entites;

namespace TaskFlow.Domain.Repositories;

public interface IProjectRepository
{
    Task<int> CreateProjectAsync(Project project);
    Task<Project?> GetProjectByIdAsync(int id);
    Task<IEnumerable<Project>> GetAllProjectsAsync();
    Task DeleteProjectAsync(Project project);
    Task AssignManagerToProject(ApplicationUser user, Project project);
}