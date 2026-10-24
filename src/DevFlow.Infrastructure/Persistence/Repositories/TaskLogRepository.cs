using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

namespace DevFlow.Infrastructure.Persistence.Repositories;

public class TaskLogRepository : ITaskLogRepository
{
    private readonly DevFlowDbContext _context;

    public TaskLogRepository(DevFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskLog taskLog, CancellationToken cancellationToken)
    {
        await _context.TaskLogs.AddAsync(taskLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
