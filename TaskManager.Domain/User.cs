using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain;

public class User
{
    public Guid Id {get; set;}
    
    [EmailAddress]
    public string Email {get; set;} = string.Empty;

    public string PasswordHash {get; set;} = string.Empty;
}