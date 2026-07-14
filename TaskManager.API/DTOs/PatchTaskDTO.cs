using System.ComponentModel.DataAnnotations;

public class PatchTaskDTO
{
    [MaxLength(60)]
    public string? Title{get; set;} 

    [MaxLength(120)]
    public string? Notes {get; set;} 

    public bool? IsCompleted{get; set;} 
    public DateOnly? DueDate {get; set;}
    public TimeOnly? DueTime {get; set;}
    public DateTime? ReminderAt {get; set;}
    public Guid? ProjectId {get; set;}


}