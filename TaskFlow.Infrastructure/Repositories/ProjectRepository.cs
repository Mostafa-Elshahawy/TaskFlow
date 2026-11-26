using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Domain.Exceptions;
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

    public async Task<Project?> GetProjectByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Projects
              .AsNoTracking()
              .Include(p => p.CreatedBy)
              .Include(p => p.Tasks)
              .Include(p => p.Members)
              .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetProjectByIdForMutationAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Projects
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(project => project.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetAllProjectsForUserAsync(
        string userId,
        int? organizationId = null,
        string? createdByUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Projects
            .AsNoTracking()
            .Include(p => p.CreatedBy)
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .Where(project => project.Organization.OwnerId == userId ||
                              project.Organization.Members.Any(member => member.UserId == userId));

        if (organizationId.HasValue)
            query = query.Where(project => project.OrganizationId == organizationId.Value);

        if (!string.IsNullOrWhiteSpace(createdByUserId))
            query = query.Where(project => project.CreatedByUserId == createdByUserId);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task DeleteProjectAsync(
        int projectId,
        byte[] expectedRowVersion,
        CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .IgnoreQueryFilters()
            .FirstAsync(project => project.Id == projectId, cancellationToken);

        dbContext.Entry(project).Property(entity => entity.RowVersion).OriginalValue = expectedRowVersion;
        project.isDeleted = true;
        project.UpdatedAt = DateTime.UtcNow;
        await SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateDetailsAsync(
        int projectId,
        string name,
        string description,
        byte[] expectedRowVersion,
        CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .IgnoreQueryFilters()
            .FirstAsync(project => project.Id == projectId, cancellationToken);

        dbContext.Entry(project).Property(entity => entity.RowVersion).OriginalValue = expectedRowVersion;
        project.Name = name;
        project.Description = description;
        project.UpdatedAt = DateTime.UtcNow;
        await SaveChangesAsync(cancellationToken);
    }

    public async Task AddMemberAsync(
        int projectId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .FirstAsync(project => project.Id == projectId, cancellationToken);

        var alreadyMember = await dbContext.ProjectMembers
            .AnyAsync(
                member => member.ProjectId == projectId && member.UserId == userId,
                cancellationToken);

        if (alreadyMember)
            return;

        await dbContext.ProjectMembers.AddAsync(new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            AddedAt = DateTime.UtcNow
        }, cancellationToken);

        project.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConflictException("The project was changed by another request. Reload and try again.", exception);
        }
    }
}
