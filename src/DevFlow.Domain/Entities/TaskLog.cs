using DevFlow.Domain.Common;
using DevFlow.Domain.Exceptions;

namespace DevFlow.Domain.Entities;

public class TaskLog : BaseEntity
{
    public Guid TaskItemId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal HoursLogged { get; private set; }
    public DateTime LoggedAt { get; private set; }

    public TaskLog(Guid taskItemId, Guid userId, decimal hoursLogged, DateTime loggedAt)
    {
        if (taskItemId == Guid.Empty)
        {
            throw new DomainException("Task id cannot be empty.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainException("User id cannot be empty.");
        }

        if (hoursLogged <= 0)
        {
            throw new DomainException("Hours logged must be greater than zero.");
        }

        if (loggedAt > DateTime.UtcNow)
        {
            throw new DomainException("Logged at cannot be a date in the future.");
        }

        TaskItemId = taskItemId;
        UserId = userId;
        HoursLogged = hoursLogged;
        LoggedAt = loggedAt;
    }
}
