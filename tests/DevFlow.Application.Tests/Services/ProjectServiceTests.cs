using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Services;
using DevFlow.Domain.Entities;

using Moq;

namespace DevFlow.Application.Tests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _service = new ProjectService(_projectRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange

        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsProject()
    {
        // Arrange
        var project = new Project(name: "Project Name", description: "Any description", ownerId: Guid.NewGuid());

        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(project);

        // Act
        var result = await _service.GetByIdAsync(project.Id);

        // Assert
        Assert.Same(project, result);
    }

    [Fact]
    public async Task CreateAsync_WithNonexistentOwner_ThrowsNotFoundException()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>()))
                   .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(
            name: "Project Name",
            description: "Any description",
            ownerId: Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateAsync_WithExistingOwner_CreatesProject()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>()))
                   .ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(
            name: "Project Name",
            description: "Any description",
            ownerId: Guid.NewGuid());

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _projectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithValidPagination_KeepsValues()
    {
        // Arrange
        var page = 2;
        var pageSize = 20;

        // Act
        var result = await _service.GetAllAsync(page, pageSize);

        // Assert
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_WithPageBelowFirstPage_UsesFirstPage()
    {
        // Arrange
        var page = 0;
        var pageSize = 20;

        // Act
        var result = await _service.GetAllAsync(page, pageSize);

        // Assert
        Assert.Equal(1, result.Page);
        Assert.Equal(pageSize, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_WithPageSizeAboveMax_UsesMaxPageSize()
    {
        // Arrange
        var page = 2;
        var pageSize = 51;

        // Act
        var result = await _service.GetAllAsync(page, pageSize);

        // Assert
        Assert.Equal(page, result.Page);
        Assert.Equal(50, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_WithPageSizeBelowMin_UsesDefaultPageSize()
    {
        // Arrange
        var page = 2;
        var pageSize = 0;

        // Act
        var result = await _service.GetAllAsync(page, pageSize);

        // Assert
        Assert.Equal(page, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingId_UpdatesProject()
    {
        // Arrange
        var project = new Project(name: "Project Name", description: "Any description", ownerId: Guid.NewGuid());

        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(project.Id))
                           .ReturnsAsync(project);

        // Act 
        await _service.UpdateAsync(
            id: project.Id,
            name: "Project Updated",
            description: "Updated description");

        // Assert
        Assert.Equal("Project Updated", project.Name);
        Assert.Equal("Updated description", project.Description);
        _projectRepositoryMock.Verify(repo => repo.UpdateAsync(project), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(
            id: Guid.NewGuid(),
            name: "Project Name",
            description: "Any Description"));
    }
}
