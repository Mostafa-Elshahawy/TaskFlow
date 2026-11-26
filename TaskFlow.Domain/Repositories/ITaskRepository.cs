using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using Task = System.Threading.Tasks.Task;

namespace TaskFlow.Domain.Repositories;

public interface ITaskRepository
{
    Task<int> Create(TaskEntity entity);
    Task<TaskEntity?> GetById(int id, CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetByIdForMutation(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskEntity>> GetFilteredTasksForUser(
        TaskFilter taskFilter,
        string userId,
        CancellationToken cancellationToken = default);
    Task SaveChangesAsync(
        TaskEntity entity,
        byte[] expectedRowVersion,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskStatusHistory>> GetStatusHistoryAsync(
        int taskId,
        CancellationToken cancellationToken = default);
}
