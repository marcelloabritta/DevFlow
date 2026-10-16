using DevFlow.Domain.Entities;
using DevFlow.Domain.Exceptions;

namespace DevFlow.Domain.Tests.Entities;

public class ProjectTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Update_WithInvalidName_ThrowsDomainException(string? name)
    {
        // Arrange
        var project = new Project(name: "Original Name", description: "Original description", ownerId: Guid.NewGuid());

        // Act & Assert
        Assert.Throws<DomainException>(() => project.Update(name!, "Any description"));
    }

    [Fact]
    public void Update_WithValidData_UpdatesProjectFields()
    {
        // Arrange
        var project = new Project(name: "Original Name", description: "Original description", ownerId: Guid.NewGuid());

        // Act 
        project.Update("Updated Name", "Updated description");

        //Assert
        Assert.Equal("Updated Name", project.Name);
        Assert.Equal("Updated description", project.Description);
        Assert.NotNull(project.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyName_ThrowsDomainException(string? name)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Project(name: name!, description: "Any description", ownerId: Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_WithEmptyOwnerId_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Project(name: "Project Name", description: "Any description", ownerId: Guid.Empty));
    }

    [Fact]
    public void Constructor_WithValidData_CreatesProject()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Act
        var project = new Project(name: "Project Name", description: "Project Description", ownerId: ownerId);

        // Assert
        Assert.Equal("Project Name", project.Name);
        Assert.Equal("Project Description", project.Description);
        Assert.Equal(ownerId, project.OwnerId);
    }
}
