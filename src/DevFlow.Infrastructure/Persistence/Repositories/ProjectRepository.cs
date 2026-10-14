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

    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Projects.CountAsync(p => !p.IsDeleted);
    }

    public async Task<IEnumerable<Project>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Projects
                             .Where(p => !p.IsDeleted)
                             .OrderByDescending(p => p.CreatedAt)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }
}
