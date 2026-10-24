using DevFlow.Application.Commands;

namespace DevFlow.API.Requests;

public record LogHoursRequest(Guid UserId, decimal Hours, DateTime LoggedAt)
{
    public LogHoursCommand ToCommand() => new LogHoursCommand(
        UserId: UserId,
        Hours: Hours,
        LoggedAt: LoggedAt
    );
}
