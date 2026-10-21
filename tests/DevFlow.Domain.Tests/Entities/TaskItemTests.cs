using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;
using DevFlow.Domain.Exceptions;

namespace DevFlow.Domain.Tests.Entities;

public class TaskItemTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidTitle_ThrowsDomainException(string? title)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateTaskItem(title: title!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    // int: attribute arguments can't be decimal; C# converts it when calling CreateTaskItem
    public void Constructor_WithNonPositiveEstimatedHours_ThrowsDomainException(int estimatedHours)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateTaskItem(estimatedHours: estimatedHours));
    }

    [Theory]
    [InlineData((TaskPriority)0)]
    [InlineData((TaskPriority)(-1))]
    public void Constructor_WithInvalidPriority_ThrowsDomainException(TaskPriority priority)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateTaskItem(priority: priority));
    }

    [Fact]
    public void Constructor_WithEmptyProjectId_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateTaskItem(projectId: Guid.Empty));
    }

    [Fact]
    public void Constructor_WithValidData_CreatesTaskItem()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var taskItem = CreateTaskItem(projectId: projectId);

        // Assert
        Assert.Equal("Valid title", taskItem.Title);
        Assert.Equal(TaskPriority.Medium, taskItem.Priority);
        Assert.Equal(5m, taskItem.EstimatedHours);
        Assert.Equal(projectId, taskItem.ProjectId);
        Assert.Equal(TaskItemStatus.Backlog, taskItem.Status);
        Assert.Equal(0m, taskItem.LoggedHours);
    }

    [Fact]
    public void LogHours_CalledTwice_AccumulatesLoggedHours()
    {
        // Arrange
        var taskItem = CreateTaskItem();
        var userId = Guid.NewGuid();

        // Act
        taskItem.LogHours(userId: userId, hours: 2m, loggedAt: DateTime.UtcNow.AddHours(-1));
        taskItem.LogHours(userId: userId, hours: 3m, loggedAt: DateTime.UtcNow.AddHours(-1));

        // Assert
        Assert.Equal(5m, taskItem.LoggedHours);
    }

    [Fact]
    public void LogHours_WithValidData_ReturnsTaskLog()
    {
        // Arrange
        var taskItem = CreateTaskItem();
        var userId = Guid.NewGuid();
        var loggedAt = DateTime.UtcNow.AddHours(-1);

        // Act
        var taskLog = taskItem.LogHours(userId: userId, hours: 2m, loggedAt: loggedAt);

        // Assert
        Assert.Equal(taskItem.Id, taskLog.TaskItemId);
        Assert.Equal(userId, taskLog.UserId);
        Assert.Equal(2m, taskLog.HoursLogged);
        Assert.Equal(loggedAt, taskLog.LoggedAt);
        Assert.NotNull(taskItem.UpdatedAt);
    }

    private static TaskItem CreateTaskItem(
     string title = "Valid title",
     TaskPriority priority = TaskPriority.Medium,
     decimal estimatedHours = 5,
     Guid? projectId = null)
    {
        return new TaskItem(
            title: title,
            description: "Any description",
            priority: priority,
            estimatedHours: estimatedHours,
            projectId: projectId ?? Guid.NewGuid(),
            assignedToUserId: null,
            dueDate: null);
    }
}
