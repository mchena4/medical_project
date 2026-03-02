using System.ComponentModel.DataAnnotations;

public class CreatePatientDTO
{   
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string Dni { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; } = string.Empty;


}