using DevFlow.Application.Common;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Services;

public class ProjectService
{
    private const int MaxPageSize = 50;
    private const int FirstPage = 1;
    private const int DefaultPageSize = 10;
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

    public async Task<PagedResult<Project>> GetAllAsync(int page, int pageSize)
    {
        if (page < FirstPage)
        {
            page = FirstPage;
        }

        if (pageSize < 1)
        {
            pageSize = DefaultPageSize;
        }

        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }


        var projects = await _projectRepository.GetAllAsync(page, pageSize);
        var total = await _projectRepository.CountAsync();

        return new PagedResult<Project>(projects, page, pageSize, total);
    }

    public async Task UpdateAsync(Guid id, string name, string? description)
    {
        var project = await GetByIdAsync(id);
        project.Update(name, description);
        await _projectRepository.UpdateAsync(project);
    }
}
