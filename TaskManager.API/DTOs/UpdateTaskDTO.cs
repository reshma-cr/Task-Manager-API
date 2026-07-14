using System.ComponentModel.DataAnnotations;

public class UpdateTaskDTO
{
    [MaxLength(60)]
    public string Title{get; set;} = string.Empty;

    [MaxLength(120)]
    public string? Notes {get; set;} 

    public bool IsCompleted{get; set;} = false;
    public DateOnly? DueDate {get; set;}
    public TimeOnly? DueTime {get; set;}
    public DateTime? ReminderAt {get; set;}
    public Guid? ProjectId {get; set;}


}