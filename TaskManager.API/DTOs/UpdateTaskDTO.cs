using System.ComponentModel.DataAnnotations;

public class UpdateTaskDTO
{
    [MaxLength(60)]
    public string Title{get; set;} = string.Empty;

    [MaxLength(120)]
    public string? Notes {get; set;} 

    public bool IsCompleted{get; set;} = false;

}