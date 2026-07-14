using TaskManager.Domain;
using TaskManager.Application;
using Microsoft.EntityFrameworkCore;

public class ProjectService: IProjectService
{
    private readonly ApplicationDbContext _context;
    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Project> GetProject(Guid id, Guid userId)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        return project;
    }

    public async Task<List<Project>> GetAllProjects(Guid userId)
    {
        var projects = await _context.Projects.Where(p => p.UserId == userId).ToListAsync();
        return projects;
    }

    public async Task<Project> CreateProject(string name, Guid userId)
    {
        Project project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            UserId = userId
        };
        await _context.AddAsync(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteProject(Guid id, Guid userId)
    {
        bool found = false;
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        var tasks = await _context.TaskItems.Where(t => t.ProjectId == id).ToListAsync();
        if(project == null)
        {
            return false;
        }
        if(tasks.Any())
        {
            throw new InvalidOperationException("Project has tasks assigned");
        }
        found = true;
        _context.Remove(project);
        await _context.SaveChangesAsync();
        return found;
    }
    
    public async Task<Project> PatchProject(Guid id, string name, Guid userId)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (!string.IsNullOrWhiteSpace(name))
        {
            project.Name = name;
        }
        await _context.SaveChangesAsync();
        return project;
    }
}