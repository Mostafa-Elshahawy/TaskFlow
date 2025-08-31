using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

internal class ProjectRepository(ApplicationDBContext dBContext) : IProjectRepository
{
    public async Task<int> CreateProjectAsync(Project project)
    {
        dBContext.Add(project);
        await dBContext.SaveChangesAsync();
        return project.Id;
    }

    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await dBContext.Projects
              .AsNoTracking()
              .Include(p => p.Tasks)
              .Include(p => p.Members)
              .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        return await dBContext.Projects
            .AsNoTracking()
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .ToListAsync();
    }

    public async Task DeleteProjectAsync(Project entity)
    {
        dBContext.Remove(entity);
        await dBContext.SaveChangesAsync();
    }

    public async Task AssignManagerToProject(ApplicationUser user, Project project)
    {
        project.Managers ??= new List<ApplicationUser>();
        project.Managers.Add(user);

        user.ManagedProjects ??= new List<Project>();
        user.ManagedProjects.Add(project);

        dBContext.Update(project);
        dBContext.Update(user);

        await dBContext.SaveChangesAsync();
    }
}