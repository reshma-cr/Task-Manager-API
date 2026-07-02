namespace TaskManager.Domain;

public class Project
{
    public Guid Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public Guid UserId {get; set;}
    public User? User {get; set;}
}