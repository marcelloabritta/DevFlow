namespace DevFlow.Application.Common;

public record ProjectWithOwner(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    Guid OwnerId,
    string? OwnerName);