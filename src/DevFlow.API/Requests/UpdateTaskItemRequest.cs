using DevFlow.Application.Commands;
using DevFlow.Domain.Enums;

namespace DevFlow.API.Requests;

public record UpdateTaskItemRequest(string Title, string? Description, TaskPriority Priority, decimal EstimatedHours, DateTime? DueDate)
{
    public UpdateTaskItemCommand ToCommand() => new UpdateTaskItemCommand(
        Title: Title,
        Description: Description,
        Priority: Priority,
        EstimatedHours: EstimatedHours,
        DueDate: DueDate
    );
}
