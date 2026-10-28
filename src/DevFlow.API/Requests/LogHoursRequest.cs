using DevFlow.Application.Commands;

namespace DevFlow.API.Requests;

public record LogHoursRequest(decimal Hours, DateTime LoggedAt)
{
    public LogHoursCommand ToCommand(Guid userId) => new LogHoursCommand(
        UserId: userId,
        Hours: Hours,
        LoggedAt: LoggedAt
    );
}
