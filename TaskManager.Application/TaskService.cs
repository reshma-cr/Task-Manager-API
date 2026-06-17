using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;

namespace TaskManager.Application;
public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAllTasks()
    {
        var tasks = await _context.TaskItems.ToListAsync();
        return tasks;
    }

    public async Task<TaskItem> GetTask(Guid id)
    {
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id);
        return task;    
    }

    public async Task<TaskItem> CreateTask(string title, string description){
        TaskItem task = new TaskItem()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description
        };
        await _context.TaskItems.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteTask(Guid taskId)
    {
        var found = false;
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);
        if(task != null)
        {
            found = true;
            _context.Remove(task);
            await _context.SaveChangesAsync();
        }
        return found;
    }

    public async Task<bool> ToggleTask(Guid taskId)
    {
        var found = false;
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);
        if(task != null)
        {
            found = true;
            task.IsCompleted = !task.IsCompleted;
            await _context.SaveChangesAsync();
        }
        return found;
    }
}