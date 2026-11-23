using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using Task = System.Threading.Tasks.Task;

namespace TaskFlow.Domain.Repositories;

public interface ITaskRepository
{
    Task<int> Create(TaskEntity entity);
    Task<TaskEntity?> GetById(int id, CancellationToken cancellationToken = default);
    Task Delete(TaskEntity entity);
    Task<IEnumerable<TaskEntity>> GetFilteredTasksForUser(
        TaskFilter taskFilter,
        string userId,
        CancellationToken cancellationToken = default);
    Task SaveChanges();
    Task AssignTask(TaskEntity task);
}
