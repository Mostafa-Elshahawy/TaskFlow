using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;
using Task = TaskFlow.Domain.Entites.Task;

namespace TaskFlow.Infrastructure.Repositories;

internal class TaskRepository(ApplicationDBContext dbContext) : ITaskRepository
{
    public async Task<int> Create(Task entity)
    {
      dbContext.Tasks.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public Task Delete(Task entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetByAssigneeId(string assigneeId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetByCreatedAt(DateTime createdAt)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetByCreatedByUserId(string createdByUserId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetByDueDate(DateTime dueDate)
    {
        throw new NotImplementedException();
    }

    public Task<Task> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetByPriority(string priority)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetByProjectId(int projectId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetByStatus(string status)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetByUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public Task SaveChanges()
    {
        throw new NotImplementedException();
    }

    public Task Update(Task entity)
    {
        throw new NotImplementedException();
    }
}
