using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Extensions;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

internal class TaskRepository(ApplicationDBContext dbContext) : ITaskRepository
{
    public async Task<int> Create(TaskEntity entity)
    {
      dbContext.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<TaskEntity?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Tasks
            .Include(t => t.CreatedBy)
            .Include(t => t.Assignee)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<TaskEntity?> GetByIdForMutation(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Tasks
            .IgnoreQueryFilters()
            .Include(task => task.Project)
            .ThenInclude(project => project.Members)
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetFilteredTasksForUser(
        TaskFilter taskFilter,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Tasks
        .AsNoTracking()
        .Include(t => t.CreatedBy)
        .Include(t => t.Assignee)
        .Include(t => t.Project)
        .AsQueryable();

        query = query
            .Where(t => t.Project.Organization.OwnerId == userId ||
                        t.Project.Organization.Members.Any(member => member.UserId == userId))
            .WhereIf(taskFilter.Status.HasValue, t => t.Status == taskFilter.Status!.Value)
            .WhereIf(taskFilter.Priority.HasValue, t => t.Priority == taskFilter.Priority!.Value)
            .WhereIf(!string.IsNullOrEmpty(taskFilter.AssigneeId), t => t.AssigneeId == taskFilter.AssigneeId)
            .WhereIf(taskFilter.ProjectId.HasValue, t => t.ProjectId == taskFilter.ProjectId)
            .WhereIf(taskFilter.DueBefore.HasValue, t => t.DueDate < taskFilter.DueBefore)
            .WhereIf(taskFilter.DueAfter.HasValue, t => t.DueDate > taskFilter.DueAfter);

        return await query.ToListAsync(cancellationToken);
    }
    public async Task SaveChangesAsync(
        TaskEntity entity,
        byte[] expectedRowVersion,
        CancellationToken cancellationToken = default)
    {
        dbContext.Entry(entity).Property(task => task.RowVersion).OriginalValue = expectedRowVersion;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConflictException("The task was changed by another request. Reload and try again.", exception);
        }
    }

    public async Task<IReadOnlyList<TaskStatusHistory>> GetStatusHistoryAsync(
        int taskId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.TaskStatusHistories
            .AsNoTracking()
            .Include(history => history.ChangedBy)
            .Where(history => history.TaskId == taskId)
            .OrderByDescending(history => history.ChangedAtUtc)
            .ThenByDescending(history => history.Id)
            .ToListAsync(cancellationToken);
    }
}
