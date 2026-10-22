using DevFlow.Application.Commands;
using DevFlow.Domain.Enums;

namespace DevFlow.API.Requests;

public record CreateTaskItemRequest(string Title, string? Description, TaskPriority Priority, decimal EstimatedHours,
    Guid ProjectId, Guid? AssignedToUserId, DateTime? DueDate)
{
    public CreateTaskItemCommand ToCommand() => new CreateTaskItemCommand(
        Title: Title,
        Description: Description,
        Priority: Priority,
        EstimatedHours: EstimatedHours,
        ProjectId: ProjectId,
        AssignedToUserId: AssignedToUserId,
        DueDate: DueDate);
}


