using System.Text.Json.Serialization;

namespace TaskManager.Domain;

public class Project
{
    public Guid Id {get; set;}
    public string Name {get; set;} = string.Empty;
    [JsonIgnore]
    public Guid UserId {get; set;}
    [JsonIgnore]
    public User? User {get; set;}
}