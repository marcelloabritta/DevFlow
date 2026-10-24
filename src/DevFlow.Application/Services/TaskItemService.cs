using DevFlow.Application.Commands;
using DevFlow.Application.Common;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Services;

public class TaskItemService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITaskLogRepository _taskLogRepository;

    public TaskItemService(IProjectRepository projectRepository, IUserRepository userRepository, ITaskItemRepository taskItemRepository, ITaskLogRepository taskLogRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _taskItemRepository = taskItemRepository;
        _taskLogRepository = taskLogRepository;
    }

    public async Task<Guid> CreateAsync(CreateTaskItemCommand command, CancellationToken cancellationToken)
    {
        var taskItem = new TaskItem(
            title: command.Title,
            description: command.Description,
            priority: command.Priority,
            estimatedHours: command.EstimatedHours,
            projectId: command.ProjectId,
            assignedToUserId: command.AssignedToUserId,
            dueDate: command.DueDate);

        var projectExists = await _projectRepository.ExistsAsync(command.ProjectId, cancellationToken);
        if (!projectExists)
        {
            throw new NotFoundException($"The project with id {command.ProjectId} does not exist.");
        }

        if (command.AssignedToUserId is not null)
        {
            var userExists = await _userRepository.ExistsAsync(command.AssignedToUserId.Value, cancellationToken);
            if (!userExists)
            {
                throw new NotFoundException($"The user with id {command.AssignedToUserId} does not exist.");
            }
        }

        await _taskItemRepository.AddAsync(taskItem, cancellationToken);
        return taskItem.Id;
    }

    public async Task<TaskItem> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(id, cancellationToken);
        if (taskItem is null)
        {
            throw new NotFoundException($"The task with id {id} does not exist.");
        }

        return taskItem;
    }

    public async Task<PagedResult<TaskItem>> GetAllAsync(Guid projectId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var projectExists = await _projectRepository.ExistsAsync(projectId, cancellationToken);
        if (!projectExists)
        {
            throw new NotFoundException($"The project with id {projectId} does not exist.");
        }

        var (normalizedPage, normalizedPageSize) = Pagination.Normalize(page, pageSize);

        var taskItems = await _taskItemRepository.GetAllAsync(projectId, normalizedPage, normalizedPageSize, cancellationToken);
        var total = await _taskItemRepository.CountAsync(projectId, cancellationToken);

        return new PagedResult<TaskItem>(taskItems, normalizedPage, normalizedPageSize, total);
    }

    public async Task UpdateAsync(Guid id, UpdateTaskItemCommand command, CancellationToken cancellationToken)
    {
        var taskItem = await GetByIdAsync(id, cancellationToken);
        taskItem.Update(
            title: command.Title,
            description: command.Description,
            priority: command.Priority,
            estimatedHours: command.EstimatedHours,
            dueDate: command.DueDate);
        await _taskItemRepository.UpdateAsync(taskItem, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var taskItem = await GetByIdAsync(id, cancellationToken);
        taskItem.MarkAsDeleted();
        await _taskItemRepository.UpdateAsync(taskItem, cancellationToken);
    }

    public async Task LogHoursAsync(Guid id, LogHoursCommand command, CancellationToken cancellationToken)
    {
        var taskItem = await GetByIdAsync(id, cancellationToken);
        var userExists = await _userRepository.ExistsAsync(command.UserId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException($"The user with id {command.UserId} does not exist.");
        }

        var taskLog = taskItem.LogHours(userId: command.UserId, hours: command.Hours, loggedAt: command.LoggedAt);
        await _taskLogRepository.AddAsync(taskLog, cancellationToken);
        await _taskItemRepository.UpdateAsync(taskItem, cancellationToken);
    }
}
