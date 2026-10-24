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
        var taskItem = CreateTaskItem();

        _taskItemRepositoryMock.Setup(repo => repo.GetByIdAsync(taskItem.Id, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(taskItem);

        // Act
        var result = await _service.GetByIdAsync(taskItem.Id, CancellationToken.None);

        // Assert
        Assert.Same(taskItem, result);
    }

    [Fact]
    public async Task GetAllAsync_WithNonexistentProject_ThrowsNotFoundException()
    {
        // Arrange
        _projectRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(false);
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAllAsync(Guid.NewGuid(), 1, 10, CancellationToken.None));
        _taskItemRepositoryMock.Verify(
            repo => repo.GetAllAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WithExistingProject_ReturnsPagedResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var page = 2;
        var pageSize = 20;
        var taskItem = CreateTaskItem(projectId);

        var taskItems = new List<TaskItem> { taskItem };

        _projectRepositoryMock.Setup(repo => repo.ExistsAsync(projectId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);
        _taskItemRepositoryMock.Setup(repo => repo.GetAllAsync(projectId, page, pageSize, It.IsAny<CancellationToken>()))
                                               .ReturnsAsync(taskItems);
        _taskItemRepositoryMock.Setup(repo => repo.CountAsync(projectId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(35);

        // Act
        var result = await _service.GetAllAsync(projectId, page, pageSize, CancellationToken.None);

        // Assert 
        Assert.Same(taskItems, result.Items);
        Assert.Equal(35, result.TotalItems);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);

    }

    [Fact]
    public async Task GetAllAsync_WithOutOfRangePagination_NormalizesValues()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _projectRepositoryMock.Setup(repo => repo.ExistsAsync(projectId, It.IsAny<CancellationToken>()))
              .ReturnsAsync(true);
        _taskItemRepositoryMock.Setup(repo => repo.GetAllAsync(projectId, 1, 50, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new List<TaskItem>());
        _taskItemRepositoryMock.Setup(repo => repo.CountAsync(projectId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(35);

        // Act
        var result = await _service.GetAllAsync(projectId, 0, 51, CancellationToken.None);

        // Assert
        Assert.Equal(1, result.Page);
        Assert.Equal(50, result.PageSize);
    }

    [Fact]
    public async Task UpdateAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        _taskItemRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(Guid.NewGuid(), CreateUpdateCommand(), CancellationToken.None));
        _taskItemRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingId_UpdatesAndSavesTaskItem()
    {
        // Arrange
        var taskItem = CreateTaskItem();
        _taskItemRepositoryMock.Setup(repo => repo.GetByIdAsync(taskItem.Id, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(taskItem);

        // Act
        await _service.UpdateAsync(taskItem.Id, CreateUpdateCommand(), CancellationToken.None);

        // Assert
        Assert.Equal("Updated title", taskItem.Title);
        Assert.Equal("Updated description", taskItem.Description);
        Assert.Equal(TaskPriority.High, taskItem.Priority);
        Assert.Equal(8m, taskItem.EstimatedHours);
        _taskItemRepositoryMock.Verify(repo => repo.UpdateAsync(taskItem, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        _taskItemRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(Guid.NewGuid(), CancellationToken.None));
        _taskItemRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_MarksTaskItemAsDeleted()
    {
        // Arrange 
        var taskItem = CreateTaskItem();
        _taskItemRepositoryMock.Setup(repo => repo.GetByIdAsync(taskItem.Id, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(taskItem);
        Assert.False(taskItem.IsDeleted);

        // Act
        await _service.DeleteAsync(taskItem.Id, CancellationToken.None);

        // Assert
        Assert.True(taskItem.IsDeleted);
        _taskItemRepositoryMock.Verify(repo => repo.UpdateAsync(taskItem, It.IsAny<CancellationToken>()), Times.Once);

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

    private static TaskItem CreateTaskItem(Guid? projectId = null)
    {
        return new TaskItem(
            "Valid title",
            "Any description",
            TaskPriority.Medium,
            5m,
            projectId ?? Guid.NewGuid(),
            null,
            null);
    }

    private static UpdateTaskItemCommand CreateUpdateCommand()
    {
        return new UpdateTaskItemCommand(
            Title: "Updated title",
            Description: "Updated description",
            Priority: TaskPriority.High,
            EstimatedHours: 8m,
            DueDate: null);
    }

}
