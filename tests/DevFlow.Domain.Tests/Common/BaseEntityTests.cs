using DevFlow.Domain.Entities;

namespace DevFlow.Domain.Tests.Common;

public class BaseEntityTests
{
    [Fact]
    public void MarkAsDeleted_WhenCalled_MarksEntityAsDeleted()
    {
        // Arrange
        var project = new Project(name: "Project Name", description: "Any description", ownerId: Guid.NewGuid());

        // Act
        project.MarkAsDeleted();

        // Assert
        Assert.True(project.IsDeleted);
        Assert.NotNull(project.UpdatedAt);
    }
}
