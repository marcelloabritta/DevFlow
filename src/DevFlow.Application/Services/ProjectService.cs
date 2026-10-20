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

    public async Task<Guid> CreateAsync(string name, string? description, Guid ownerId, CancellationToken cancellationToken)
    {
        var project = new Project(name, description, ownerId);

        var ownerExists = await _userRepository.ExistsAsync(ownerId, cancellationToken);
        if (!ownerExists)
        {
            throw new NotFoundException($"The user with id {ownerId} does not exist.");
        }

        await _projectRepository.AddAsync(project, cancellationToken);
        return project.Id;
    }

    public async Task<Project> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException($"The project with id {id} does not exist.");
        }

        return project;
    }

    public async Task<PagedResult<Project>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
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


        var projects = await _projectRepository.GetAllAsync(page, pageSize, cancellationToken);
        var total = await _projectRepository.CountAsync(cancellationToken);

        return new PagedResult<Project>(projects, page, pageSize, total);
    }

    public async Task UpdateAsync(Guid id, string name, string? description, CancellationToken cancellationToken)
    {
        var project = await GetByIdAsync(id, cancellationToken);
        project.Update(name, description);
        await _projectRepository.UpdateAsync(project, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var project = await GetByIdAsync(id, cancellationToken);
        project.MarkAsDeleted();
        await _projectRepository.UpdateAsync(project, cancellationToken);
    }
}
