using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Persistence.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly DevFlowDbContext _context;

    public TaskItemRepository(DevFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        await _context.TaskItems.AddAsync(taskItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _context.TaskItems.Where(t => t.ProjectId == projectId)
                                       .CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(Guid projectId, int page, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.TaskItems
                             .AsNoTracking()
                             .Where(t => t.ProjectId == projectId)
                             .OrderByDescending(t => t.CreatedAt)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.TaskItems.FirstOrDefaultAsync(ti => ti.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
