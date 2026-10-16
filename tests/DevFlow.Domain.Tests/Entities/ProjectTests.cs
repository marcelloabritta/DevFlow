using DevFlow.Domain.Entities;
using DevFlow.Domain.Exceptions;

namespace DevFlow.Domain.Tests.Entities;

public class ProjectTests
{
    [Fact]
    public void Update_WithEmptyName_ThrowsDomainException()
    {
        // Arrange
        var project = new Project(name: "Original Name", description: "Original description", ownerId: Guid.NewGuid());

        // Act & Assert
        Assert.Throws<DomainException>(() => project.Update("", "Any description"));
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
}
