namespace TaskManager.Domain;
public class TaskItem
{
    public Guid Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public bool IsCompleted {get; set;} = false;
    public Guid UserId {get; set;}
    public User? User {get; set;}
}