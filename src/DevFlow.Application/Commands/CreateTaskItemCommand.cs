using DevFlow.Domain.Enums;

namespace DevFlow.Application.Commands;

public record CreateTaskItemCommand(
    string Title, string? Description, TaskPriority Priority, decimal EstimatedHours,
    Guid ProjectId, Guid? AssignedToUserId, DateTime? DueDate);

