using DevFlow.Application.Common;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Services;

public class ProjectService
{
    private const int ProjectsCacheMinutes = 5;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;

    public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository, ICacheService cacheService)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _cacheService = cacheService;
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

    public async Task<PagedResult<ProjectWithOwner>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var (normalizedPage, normalizedPageSize) = Pagination.Normalize(page, pageSize);

        var cacheKey = $"projects:page={normalizedPage}:size={normalizedPageSize}";
        var cached = await _cacheService.GetAsync<PagedResult<ProjectWithOwner>>(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var projects = await _projectRepository.GetAllAsync(normalizedPage, normalizedPageSize, cancellationToken);
        var total = await _projectRepository.CountAsync(cancellationToken);
        var assignedIds = projects.Select(p => p.OwnerId)
                                  .Distinct()
                                  .ToList();

        var namesById = new Dictionary<Guid, string>();

        if (assignedIds.Count > 0)
        {
            var users = await _userRepository.GetByIdsAsync(assignedIds, cancellationToken);
            namesById = users.ToDictionary(u => u.Id, u => u.Name);
        }

        var items = projects.Select(p => new ProjectWithOwner(p.Id, p.Name, p.Description, p.CreatedAt, p.UpdatedAt, p.OwnerId, namesById.GetValueOrDefault(p.OwnerId)));

        var result = new PagedResult<ProjectWithOwner>(items.ToList(), normalizedPage, normalizedPageSize, total);

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(ProjectsCacheMinutes), cancellationToken);

        return result;
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
