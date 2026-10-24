using DevFlow.Domain.Enums;

namespace DevFlow.API.Requests;

public record ChangeStatusRequest(TaskItemStatus Status);
