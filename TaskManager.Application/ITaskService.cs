using TaskManager.Domain;

namespace TaskManager.Application;

public interface ITaskService
{
    Task<TaskItem> CreateTask(string title, string description, Guid userId);

    Task<List<TaskItem>> GetAllTasks(Guid userid, bool? status=null, string? search=null);

    Task<TaskItem> GetTask(Guid userid, Guid id);

    Task<bool> DeleteTask(Guid taskId);

    Task<bool> ToggleTask(Guid taskId);

    Task<TaskItem> UpdateTask(Guid id, string title, string description, bool isCompleted);

    Task<TaskItem> PatchTask(Guid id, string? title, string? description, bool? iscompleted);
}
