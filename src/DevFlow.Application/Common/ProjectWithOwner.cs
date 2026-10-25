using DevFlow.Domain.Entities;

namespace DevFlow.Application.Common;

public record ProjectWithOwner(Project Project, string? OwnerName);
