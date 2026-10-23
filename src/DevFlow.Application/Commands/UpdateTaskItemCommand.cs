using DevFlow.Domain.Enums;

namespace DevFlow.Application.Commands;

public record UpdateTaskItemCommand(string Title, string? Description, TaskPriority Priority, decimal EstimatedHours, DateTime? DueDate);
