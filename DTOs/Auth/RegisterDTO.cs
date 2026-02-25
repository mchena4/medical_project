using System.ComponentModel.DataAnnotations;

namespace MedicalClinicAPI.DTOs.Auth;

public class RegisterDTO
{
    [Required][EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public string Dni { get; set; } = string.Empty;
}