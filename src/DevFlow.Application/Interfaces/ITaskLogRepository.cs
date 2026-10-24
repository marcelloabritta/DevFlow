using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces;

public interface ITaskLogRepository
{
    Task AddAsync(TaskLog taskLog, CancellationToken cancellationToken);
}
