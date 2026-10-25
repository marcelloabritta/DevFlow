using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}
