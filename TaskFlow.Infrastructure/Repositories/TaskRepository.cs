using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Extensions;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
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

    public async Task Delete(TaskEntity entity)
    {
        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<TaskEntity?> GetById(int id)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        return task;
    }

    public async Task<IEnumerable<TaskEntity>> GetFilteredTasks(TaskFilter taskFilter)
    {
        var query = dbContext.Tasks
        .AsNoTracking()
        .Include(t => t.Assignee)
        .Include(t => t.Project)
        .AsQueryable();

        query = query
            .WhereIf(taskFilter.Status.HasValue, t => t.Status == taskFilter.Status!.Value)
            .WhereIf(taskFilter.Priority.HasValue, t => t.Priority == taskFilter.Priority!.Value)
            .WhereIf(!string.IsNullOrEmpty(taskFilter.AssigneeId), t => t.AssigneeId == taskFilter.AssigneeId)
            .WhereIf(taskFilter.ProjectId.HasValue, t => t.ProjectId == taskFilter.ProjectId)
            .WhereIf(taskFilter.DueBefore.HasValue, t => t.DueDate < taskFilter.DueBefore)
            .WhereIf(taskFilter.DueAfter.HasValue, t => t.DueDate > taskFilter.DueAfter);

        return await query.ToListAsync();
    }
    public Task SaveChanges() => dbContext.SaveChangesAsync();
}
