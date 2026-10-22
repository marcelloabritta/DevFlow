using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces;

public interface ITaskItemRepository
{
    Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken);
}
