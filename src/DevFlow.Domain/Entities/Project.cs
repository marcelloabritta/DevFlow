using DevFlow.Domain.Common;
using DevFlow.Domain.Exceptions;

namespace DevFlow.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }

    public Project(string name, string? description, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Project name is required.");
        }

        if (ownerId == Guid.Empty)
        {
            throw new DomainException("Owner id cannot be empty.");
        }

        Name = name;
        Description = description;
        OwnerId = ownerId;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Project name is required.");
        }

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}
