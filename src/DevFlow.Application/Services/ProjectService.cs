using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Services;

public class ProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateAsync(string name, string? description, Guid ownerId)
    {
        var project = new Project(name, description, ownerId);
        await _repository.AddAsync(project);
        return project.Id;
    }
}
