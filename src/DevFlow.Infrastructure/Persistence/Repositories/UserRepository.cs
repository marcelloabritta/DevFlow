using DevFlow.Application.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DevFlowDbContext _context;

    public UserRepository(DevFlowDbContext context)
    {
        _context = context;
    }
    public async Task<bool> ExistsAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }
}
