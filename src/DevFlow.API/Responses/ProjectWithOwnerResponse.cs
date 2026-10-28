using DevFlow.Application.Common;

namespace DevFlow.API.Responses;

public record ProjectWithOwnerResponse(Guid Id, string Name, string? Description, DateTime CreatedAt, DateTime? UpdatedAt, Guid OwnerId, string? OwnerName)
{
    public static ProjectWithOwnerResponse FromModel(ProjectWithOwner item)
    {
        var project = item;

        return new ProjectWithOwnerResponse(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedAt,
            project.UpdatedAt,
            project.OwnerId,
            item.OwnerName);
    }
}
