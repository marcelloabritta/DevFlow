using DevFlow.Application.Common;
using DevFlow.Domain.Enums;

namespace DevFlow.API.Responses;

public record TaskItemWithAssigneeResponse(
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
    DateTime? UpdatedAt,
    string? AssigneeName)
{
    public static TaskItemWithAssigneeResponse FromModel(TaskItemWithAssignee item)
    {
        var taskItem = item.TaskItem;

        return new TaskItemWithAssigneeResponse(
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
            taskItem.UpdatedAt,
            item.UserName);
    }
}

