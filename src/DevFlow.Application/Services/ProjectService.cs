using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Services;

public class ProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
    }

    public async Task<Guid> CreateAsync(string name, string? description, Guid ownerId)
    {
        var project = new Project(name, description, ownerId);

        var ownerExists = await _userRepository.ExistsAsync(ownerId);
        if (!ownerExists)
        {
            throw new NotFoundException($"The user with id {ownerId} does not exist.");
        }

        await _projectRepository.AddAsync(project);
        return project.Id;
    }

    public async Task<Project> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null)
        {
            throw new NotFoundException($"The project with id {id} does not exist.");
        }

        return project;
    }

    public async Task<IEnumerable<Project>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (pageSize > 50)
        {
            pageSize = 50;
        }

        return await _projectRepository.GetAllAsync(page, pageSize);
    }
}
