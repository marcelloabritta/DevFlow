using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

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
}
