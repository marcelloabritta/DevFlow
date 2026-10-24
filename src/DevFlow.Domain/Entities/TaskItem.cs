using DevFlow.Domain.Common;
using DevFlow.Domain.Enums;
using DevFlow.Domain.Exceptions;

namespace DevFlow.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TaskPriority Priority { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public decimal EstimatedHours { get; private set; }
    public decimal LoggedHours { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public DateTime? DueDate { get; private set; }

    public TaskItem(string title, string? description, TaskPriority priority, decimal estimatedHours, Guid projectId, Guid? assignedToUserId, DateTime? dueDate)
    {
        Validate(title, priority, estimatedHours);

        if (projectId == Guid.Empty)
        {
            throw new DomainException("Project id cannot be empty.");
        }

        Title = title;
        Description = description;
        Priority = priority;
        Status = TaskItemStatus.Backlog;
        EstimatedHours = estimatedHours;
        LoggedHours = 0;
        ProjectId = projectId;
        AssignedToUserId = assignedToUserId;
        DueDate = dueDate;
    }

    public TaskLog LogHours(Guid userId, decimal hours, DateTime loggedAt)
    {
        var log = new TaskLog(Id, userId, hours, loggedAt);

        LoggedHours += hours;
        UpdatedAt = DateTime.UtcNow;

        return log;
    }

    public void Update(string title, string? description, TaskPriority priority, decimal estimatedHours, DateTime? dueDate)
    {
        Validate(title, priority, estimatedHours);
        Title = title;
        Description = description;
        Priority = priority;
        EstimatedHours = estimatedHours;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(TaskItemStatus newStatus)
    {
        if (!Enum.IsDefined(newStatus))
        {
            throw new DomainException("Invalid task status.");
        }

        if (Status == newStatus)
        {
            throw new DomainException("The task is already in this status.");
        }

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string title, TaskPriority priority, decimal estimatedHours)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("The task title is required.");
        }

        if (!Enum.IsDefined(priority))
        {
            throw new DomainException("Invalid task priority.");
        }

        if (estimatedHours <= 0)
        {
            throw new DomainException("Estimated hours must be greater than zero.");
        }
    }


}
