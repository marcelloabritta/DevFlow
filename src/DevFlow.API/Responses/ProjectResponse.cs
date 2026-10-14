namespace DevFlow.API.Responses;

public record ProjectResponse(Guid Id, string Name, string? Description, DateTime CreatedAt, DateTime? UpdatedAt, Guid OwnerId);
