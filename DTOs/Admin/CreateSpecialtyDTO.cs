using System.ComponentModel.DataAnnotations;

public class CreateSpecialtyDTO
{
    [Required]
    public string Name { get; set; } = string.Empty;
}