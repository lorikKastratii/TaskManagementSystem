using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;
    private readonly ILogger<TaskRepository> _logger;

    public TaskRepository(TaskDbContext context, ILogger<TaskRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        _logger.LogDebug("Retrieving all tasks from database");
        var tasks = await _context.Tasks.ToListAsync();

        _logger.LogDebug("Retrieved {Count} tasks from database", tasks.Count);
        return tasks;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Retrieving task with ID: {TaskId} from database", id);
        var task = await _context.Tasks.FindAsync(id);
        
        if (task == null)
        {
            _logger.LogDebug("Task with ID: {TaskId} not found in database", id);
        }
        
        return task;
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _logger.LogDebug("Adding new task to database: {TaskId}", task.Id);
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        _logger.LogDebug("Task {TaskId} saved to database", task.Id);
        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        _logger.LogDebug("Updating task in database: {TaskId}", task.Id);
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        _logger.LogDebug("Task {TaskId} updated in database", task.Id);
        return task;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogDebug("Deleting task with ID: {TaskId} from database", id);
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            _logger.LogDebug("Task with ID: {TaskId} not found for deletion", id);
            return false;
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        _logger.LogDebug("Task {TaskId} deleted from database", id);
        return true;
    }
}
