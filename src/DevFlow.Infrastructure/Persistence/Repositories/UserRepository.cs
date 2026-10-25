using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DevFlowDbContext _context;

    public UserRepository(DevFlowDbContext context)
    {
        _context = context;
    }
    public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await _context.Users.AsNoTracking()
                                   .Where(u => ids.Contains(u.Id))
                                   .ToListAsync(cancellationToken);
    }
}
