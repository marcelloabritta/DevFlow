using DevFlow.Domain.Common;

namespace DevFlow.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }

    public Project(string name, string? description, Guid ownerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Owner id cannot be empty.");
        }

        Name = name;
        Description = description;
        OwnerId = ownerId;
    }
}
