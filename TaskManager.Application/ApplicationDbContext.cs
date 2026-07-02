using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<TaskItem> TaskItems {get; set;}
    public DbSet<User> Users {get; set;}
    public DbSet<Project> Projects {get; set;}
}