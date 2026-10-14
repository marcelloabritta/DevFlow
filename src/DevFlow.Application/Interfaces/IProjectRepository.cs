using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces;

public interface IProjectRepository
{
    Task AddAsync(Project project);
    Task<Project?> GetByIdAsync(Guid id);
    Task<IEnumerable<Project>> GetAllAsync(int page, int pageSize);
    Task<int> CountAsync();
}
