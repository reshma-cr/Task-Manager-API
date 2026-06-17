using System.ComponentModel.DataAnnotations;

public class CreateTaskDTO
{
    [Required]
    [MaxLength(60)]
    public string Title{get; set;} = string.Empty;
    
    [MaxLength(120)]
    public string Description {get; set;} = string.Empty;

}