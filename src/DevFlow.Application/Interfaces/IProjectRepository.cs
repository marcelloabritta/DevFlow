using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces;

public interface IProjectRepository
{
    Task AddAsync(Project project, CancellationToken cancellationToken);
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Project>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Project project, CancellationToken cancellationToken);
}
