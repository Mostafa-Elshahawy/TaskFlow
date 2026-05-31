using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

internal class ProjectRepository(ApplicationDBContext dbContext) : IProjectRepository
{
    public async Task<int> CreateProjectAsync(Project project)
    {
        dbContext.Add(project);
        await dbContext.SaveChangesAsync();
        return project.Id;
    }

    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await dbContext.Projects
              .AsNoTracking()
              .Include(p => p.CreatedBy)
              .Include(p => p.Tasks)
              .Include(p => p.Members)
              .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.CreatedBy)
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .ToListAsync();
    }

    public async Task DeleteProjectAsync(Project entity)
    {
        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project entity)
    {
        dbContext.Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task AssignManagerToProject(ApplicationUser user, Project project)
    {
        project.Members.Add(new ProjectMember
        {
            ProjectId = project.Id,
            UserId = user.Id,
        });

        await dbContext.SaveChangesAsync();
    }
}