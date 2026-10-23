using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces;

public interface ITaskItemRepository
{
    Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken);
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<TaskItem>> GetAllAsync(Guid projectId, int page, int pageSize, CancellationToken cancellationToken);
    Task<int> CountAsync(Guid projectId, CancellationToken cancellationToken);
}
