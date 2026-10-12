namespace DevFlow.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsAsync(Guid userId);
}
