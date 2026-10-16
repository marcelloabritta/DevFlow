using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Services;
using DevFlow.Domain.Entities;

using Moq;

namespace DevFlow.Application.Tests.Services;

public class ProjectServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WithNonexistentId_ThrowsNotFoundException()
    {
        // Arrange
        var projectRepositoryMock = new Mock<IProjectRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var service = new ProjectService(projectRepositoryMock.Object, userRepositoryMock.Object);

        projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsProject()
    {
        // Arrange
        var projectRepositoryMock = new Mock<IProjectRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var service = new ProjectService(projectRepositoryMock.Object, userRepositoryMock.Object);
        var project = new Project(name: "Project Name", description: "Any description", ownerId: Guid.NewGuid());

        projectRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(project);

        // Act
        var result = await service.GetByIdAsync(project.Id);

        // Assert
        Assert.Same(project, result);
    }
}
