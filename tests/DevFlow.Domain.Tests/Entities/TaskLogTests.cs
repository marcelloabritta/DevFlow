using DevFlow.Domain.Entities;
using DevFlow.Domain.Exceptions;

namespace DevFlow.Domain.Tests.Entities;

public class TaskLogTests
{
    [Fact]
    public void Constructor_WithEmptyTaskItemId_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateTaskLog(taskItemId: Guid.Empty));
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateTaskLog(userId: Guid.Empty));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    // int: attribute arguments can't be decimal; C# converts it when calling CreateTaskLog
    public void Constructor_WithNonPositiveHoursLogged_ThrowsDomainException(int hoursLogged)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateTaskLog(hoursLogged: hoursLogged));
    }

    [Fact]
    public void Constructor_WithFutureLoggedAt_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateTaskLog(loggedAt: DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Constructor_WithValidData_CreatesTaskLog()
    {
        // Arrange
        var taskItemId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var loggedAt = DateTime.UtcNow.AddHours(-1);

        // Act
        var taskLog = CreateTaskLog(taskItemId: taskItemId, userId: userId, loggedAt: loggedAt);

        // Assert
        Assert.Equal(taskItemId, taskLog.TaskItemId);
        Assert.Equal(userId, taskLog.UserId);
        Assert.Equal(loggedAt, taskLog.LoggedAt);
        Assert.Equal(5m, taskLog.HoursLogged);
    }

    private static TaskLog CreateTaskLog(
        Guid? taskItemId = null,
        Guid? userId = null,
        decimal hoursLogged = 5m,
        DateTime? loggedAt = null)
    {
        return new TaskLog(
            taskItemId: taskItemId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            hoursLogged: hoursLogged,
            loggedAt: loggedAt ?? DateTime.UtcNow);
    }
}
