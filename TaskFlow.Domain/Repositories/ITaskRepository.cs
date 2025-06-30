using TaskFlow.Domain.Entites;
using Task = TaskFlow.Domain.Entites.Task;

namespace TaskFlow.Domain.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<Task>> GetAll();
    Task<Task> GetById(int id);
    Task<int> Create(Task entity);
    Task Update(Task entity);
    Task Delete(Task entity);
    Task<IEnumerable<Task>> GetByProjectId(int projectId);
    Task<IEnumerable<Task>> GetByUserId(string userId);
    Task<IEnumerable<Task>> GetByStatus(string status);
    Task<IEnumerable<Task>> GetByAssigneeId(string assigneeId);
    Task<IEnumerable<Task>> GetByDueDate(DateTime dueDate);
    Task<IEnumerable<Task>> GetByPriority(string priority);
    Task<IEnumerable<Task>> GetByCreatedByUserId(string createdByUserId);
    Task<IEnumerable<Task>> GetByCreatedAt(DateTime createdAt);
    Task SaveChanges();
}
