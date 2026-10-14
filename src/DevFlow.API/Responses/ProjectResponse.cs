using DevFlow.Domain.Entities;

namespace DevFlow.API.Responses;

public record ProjectResponse(Guid Id, string Name, string? Description, DateTime CreatedAt, DateTime? UpdatedAt, Guid OwnerId)
{
    public static ProjectResponse FromEntity(Project project)
    {
        return new ProjectResponse(project.Id, project.Name, project.Description, project.CreatedAt, project.UpdatedAt, project.OwnerId);
    }
}
