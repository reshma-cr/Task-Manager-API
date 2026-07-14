using TaskManager.Domain;

namespace TaskManager.Application;
public interface INLTaskService
{
    Task<TaskItem> NLToJson(string input, Guid userId);
}