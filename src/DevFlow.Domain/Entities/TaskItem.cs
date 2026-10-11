using DevFlow.Domain.Common;
using DevFlow.Domain.Enums;

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
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        if (!Enum.IsDefined(priority))
        {
            throw new ArgumentException("Priority is not valid.");
        }
        if (estimatedHours <= 0)
        {
            throw new ArgumentException("Estimated hours must be a number more than 0");
        }
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException("Project id cannot be empty.");
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

    public void LogHours(decimal hours)
    {
        if (hours <= 0)
        {
            throw new ArgumentException("Logged hours must be a number more than 0");
        }

        LoggedHours += hours;
        UpdatedAt = DateTime.UtcNow;
    }
}
