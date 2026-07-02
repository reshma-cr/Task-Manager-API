namespace TaskManager.Domain;
public class TaskItem
{
    public Guid Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string? Notes {get; set;}
    public bool IsCompleted {get; set;} = false;
    public DateOnly? DueDate {get; set;}
    public TimeOnly? DueTime {get; set;}
    public DateTime? ReminderAt {get; set;}
    public Guid UserId {get; set;}
    public User? User {get; set;}
    public Guid? ProjectId {get; set;}
    public Project? Project {get; set;}
}