using TaskManager.Domain;

namespace TaskManager.Application;

public interface ITaskService
{
    Task<TaskItem> CreateTask(string title, string? notes, Guid userId);

    Task<List<TaskItem>> GetAllTasks(Guid userid, bool? status=null, string? search=null);

    Task<TaskItem> GetTask(Guid userid, Guid id);

    Task<bool> DeleteTask(Guid taskId, Guid userId);

    Task<bool> ToggleTask(Guid taskId, Guid UserId);

    Task<TaskItem> UpdateTask(Guid id, Guid userId, string title, string? notes, bool isCompleted);

    Task<TaskItem> PatchTask(Guid id, Guid userId, string? title, string? description, bool? iscompleted);
}
