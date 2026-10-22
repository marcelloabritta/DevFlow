using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly DevFlowDbContext _context;

    public ProjectRepository(DevFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await _context.Projects.CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _context.Projects.AnyAsync(p => p.Id == projectId, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Projects
                             .AsNoTracking()
                             .OrderByDescending(p => p.CreatedAt)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
