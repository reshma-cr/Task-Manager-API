using System.Security;
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

    public async Task<List<TaskItem>> GetAllTasks(Guid userId, bool? status=null, string? search=null)
    {
        var query = _context.TaskItems.AsQueryable().Where(t => t.UserId == userId);
        if(status != null)
        {
            query = query.Where(t => t.IsCompleted == status);
            
        }
        if(search != null)
        {
            query = query.Where(t => t.Title.Contains(search));
        }
        return await query.ToListAsync();
    }

    public async Task<TaskItem> GetTask(Guid userid, Guid id)
    {
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userid);
        return task;    
    }

    public async Task<TaskItem> CreateTask(string title, string? notes, Guid userId, DateOnly? dueDate, TimeOnly? dueTime, DateTime? reminderAt, Guid? projectId){
        TaskItem task = new TaskItem()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Notes = notes,
            UserId = userId,
            DueDate = dueDate,
            DueTime = dueTime,
            ReminderAt = reminderAt,
            ProjectId = projectId
        };

        await _context.TaskItems.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteTask(Guid taskId, Guid userId)
    {
        var found = false;
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        if(task != null)
        {
            found = true;
            _context.Remove(task);
            await _context.SaveChangesAsync();
        }
        return found;
    }

    public async Task<bool> ToggleTask(Guid taskId, Guid userId)
    {
        var found = false;
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        if(task != null)
        {
            found = true;
            task.IsCompleted = !task.IsCompleted;
            await _context.SaveChangesAsync();
        }
        return found;
    }

    public async Task<TaskItem> UpdateTask(Guid id, Guid userId, string title, string? notes, bool isCompleted, DateOnly? dueDate, TimeOnly? dueTime, DateTime? reminderAt, Guid? projectId)
    {
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        task.Title = title;
        task.Notes = notes;
        task.IsCompleted = isCompleted;
        task.DueDate = dueDate;
        task.DueTime = dueTime;
        task.ReminderAt = reminderAt;
        task.ProjectId = projectId;
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> PatchTask(Guid id, Guid userId, string? title, string? notes, bool? status, DateOnly? dueDate, TimeOnly? dueTime, DateTime? reminderAt, Guid? projectId)
    {
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (!String.IsNullOrEmpty(title))
        {
            task.Title = title;
        }
        if (!String.IsNullOrEmpty(notes))
        {
            task.Notes = notes;
        }
        if(status != null)
        {
            task.IsCompleted = status.Value;
        }
        if (dueDate != null)
        {
            task.DueDate = dueDate;
        }
        if (dueTime != null)
        {
            task.DueTime = dueTime;
        }
        if (reminderAt != null)
        {
            task.ReminderAt = reminderAt;
        }
        if (projectId != null)
        {
            task.ProjectId = projectId;
        }
        await _context.SaveChangesAsync();
        return task;
    }
}