using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using Task = System.Threading.Tasks.Task;

namespace TaskFlow.Domain.Repositories;

public interface ITaskRepository
{
    Task<int> Create(TaskEntity entity);
    Task<TaskEntity?> GetById(int id);
    Task Delete(TaskEntity entity);
    Task<IEnumerable<TaskEntity>> GetFilteredTasks(TaskFilter taskFilter);
    Task SaveChanges();
}
