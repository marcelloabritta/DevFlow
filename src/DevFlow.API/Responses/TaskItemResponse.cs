using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;

namespace DevFlow.API.Responses;

public record TaskItemResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskPriority Priority,
    TaskItemStatus Status,
    decimal EstimatedHours,
    decimal LoggedHours,
    Guid ProjectId,
    Guid? AssignedToUserId,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public static TaskItemResponse FromEntity(TaskItem taskItem)
    {
        return new TaskItemResponse(
            taskItem.Id,
            taskItem.Title,
            taskItem.Description,
            taskItem.Priority,
            taskItem.Status,
            taskItem.EstimatedHours,
            taskItem.LoggedHours,
            taskItem.ProjectId,
            taskItem.AssignedToUserId,
            taskItem.DueDate,
            taskItem.CreatedAt,
            taskItem.UpdatedAt);
    }
}
