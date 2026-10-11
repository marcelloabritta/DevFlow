using DevFlow.Domain.Common;

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
            throw new ArgumentException("Task id cannot be empty.");
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.");
        }
        if (hoursLogged <= 0)
        {
            throw new ArgumentException("Hours logged must be a number more than 0.");
        }

        if (loggedAt > DateTime.UtcNow)
        {
            throw new ArgumentException("Logged at cannot be a date in the future.");
        }

        TaskItemId = taskItemId;
        UserId = userId;
        HoursLogged = hoursLogged;
        LoggedAt = loggedAt;
    }
}
