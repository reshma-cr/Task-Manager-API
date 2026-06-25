using System.ComponentModel.DataAnnotations;

public class UpdateTaskDTO
{
    [MaxLength(60)]
    public string Title{get; set;} = string.Empty;

    [MaxLength(120)]
    public string Description {get; set;} = string.Empty;

    public bool IsCompleted{get; set;} = false;

}