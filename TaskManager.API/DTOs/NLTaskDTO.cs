using System.ComponentModel.DataAnnotations;

public class NLTaskDTO
{
    [Required]
    public string Input {get; set;} = String.Empty;
}