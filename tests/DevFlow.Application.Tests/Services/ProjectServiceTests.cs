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
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsProject()
    {
        // Arrange
        var project = new Project(name: "Project Name", description: "Any description", ownerId: Guid.NewGuid());

        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(project);

        // Act
        var result = await _service.GetByIdAsync(project.Id, CancellationToken.None);

        // Assert
        Assert.Same(project, result);
    }

    [Fact]
    public async Task CreateAsync_WithNonexistentOwner_ThrowsNotFoundException()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(
            name: "Project Name",
            description: "Any description",
            ownerId: Guid.NewGuid(),
            cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WithExistingOwner_CreatesProject()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(
            name: "Project Name",
            description: "Any description",
            ownerId: Guid.NewGuid(),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _projectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithValidPagination_KeepsValues()
    {
        // Arrange
        var page = 2;
        var pageSize = 20;

        // Act
        var result = await _service.GetAllAsync(page, pageSize, CancellationToken.None);

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
        var result = await _service.GetAllAsync(page, pageSize, CancellationToken.None);

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
        var result = await _service.GetAllAsync(page, pageSize, CancellationToken.None);

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
        var result = await _service.GetAllAsync(page, pageSize, CancellationToken.None);

        // Assert
        Assert.Equal(page, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task UpdateAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(
            id: Guid.NewGuid(),
            name: "Project Name",
            description: "Any description",
            cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WithExistingId_UpdatesProject()
    {
        // Arrange
        var project = new Project(name: "Project Name", description: "Any description", ownerId: Guid.NewGuid());

        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(project);

        // Act
        await _service.UpdateAsync(
            id: project.Id,
            name: "Project Updated",
            description: "Updated description",
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal("Project Updated", project.Name);
        Assert.Equal("Updated description", project.Description);
        _projectRepositoryMock.Verify(repo => repo.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(id: Guid.NewGuid(), cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_MarksProjectAsDeleted()
    {
        // Arrange
        var project = new Project(name: "Project Name", description: "Any description", ownerId: Guid.NewGuid());

        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(project);

        // Act
        await _service.DeleteAsync(id: project.Id, cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(project.IsDeleted);
        _projectRepositoryMock.Verify(repo => repo.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once);
    }
}
