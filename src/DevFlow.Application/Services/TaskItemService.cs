using DevFlow.Application.Commands;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Services;

public class TaskItemService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskItemRepository _taskItemRepository;

    public TaskItemService(IProjectRepository projectRepository, IUserRepository userRepository, ITaskItemRepository taskItemRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _taskItemRepository = taskItemRepository;
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
}
