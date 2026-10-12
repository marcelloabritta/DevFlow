using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

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
}
