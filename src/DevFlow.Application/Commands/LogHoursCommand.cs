namespace DevFlow.Application.Commands;

public record LogHoursCommand(Guid UserId, decimal Hours, DateTime LoggedAt);
