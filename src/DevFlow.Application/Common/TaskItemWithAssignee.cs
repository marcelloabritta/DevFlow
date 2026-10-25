using DevFlow.Domain.Entities;

namespace DevFlow.Application.Common;

public record TaskItemWithAssignee(TaskItem TaskItem, string? UserName);
