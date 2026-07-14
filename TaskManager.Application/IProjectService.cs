using TaskManager.Domain;

namespace TaskManager.Application;

public interface IProjectService
{
    Task<Project> GetProject(Guid id, Guid userId);
    Task<List<Project>> GetAllProjects(Guid userId);
    Task<Project> CreateProject(string Name, Guid userId);
    Task<bool> DeleteProject(Guid id, Guid userId);
    Task<Project> PatchProject(Guid id, string name, Guid userId);
}