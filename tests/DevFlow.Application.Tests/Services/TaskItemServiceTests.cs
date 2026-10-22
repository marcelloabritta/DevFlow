using DevFlow.Application.Commands;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Services;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;

using Moq;

namespace DevFlow.Application.Tests.Services;

public class TaskItemServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly TaskItemService _service;

    public TaskItemServiceTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
        _service = new TaskItemService(_projectRepositoryMock.Object, _userRepositoryMock.Object, _taskItemRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithNonexistentProject_ThrowsNotFoundException()
    {
        // Arrange
        _projectRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(CreateCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WithNonexistentAssignee_ThrowsNotFoundException()
    {
        // Arrange
        _projectRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(true);

        _userRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(CreateCommand(assignedToUserId: Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WithoutAssignee_CreatesTaskItem()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _projectRepositoryMock.Setup(repo => repo.ExistsAsync(projectId, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(CreateCommand(projectId: projectId), CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _taskItemRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithExistingAssignee_CreatesTaskItem()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var assignedToUserId = Guid.NewGuid();

        _projectRepositoryMock.Setup(repo => repo.ExistsAsync(projectId, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(true);

        _userRepositoryMock.Setup(repo => repo.ExistsAsync(assignedToUserId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(CreateCommand(projectId: projectId, assignedToUserId: assignedToUserId), CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _taskItemRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        _taskItemRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsTaskItem()
    {
        // Arrange
        var taskItem = new TaskItem(
            "Valid title",
            "Any description",
            TaskPriority.Medium,
            5m,
            Guid.NewGuid(),
            null,
            null);

        _taskItemRepositoryMock.Setup(repo => repo.GetByIdAsync(taskItem.Id, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(taskItem);

        // Act
        var result = await _service.GetByIdAsync(taskItem.Id, CancellationToken.None);

        // Assert
        Assert.Same(taskItem, result);
    }

    private static CreateTaskItemCommand CreateCommand(
    Guid? projectId = null,
    Guid? assignedToUserId = null)
    {
        return new CreateTaskItemCommand(
        Title: "Valid title",
        Description: "Any description",
        Priority: TaskPriority.Medium,
        EstimatedHours: 5m,
        ProjectId: projectId ?? Guid.NewGuid(),
        AssignedToUserId: assignedToUserId,
        DueDate: null);
    }

}
