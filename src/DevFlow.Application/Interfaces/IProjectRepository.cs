using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces;

public interface IProjectRepository
{
    Task AddAsync(Project project);
}
