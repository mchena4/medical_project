using System.ComponentModel.DataAnnotations;

namespace MedicalClinicAPI.DTOs.Users;

public class CreateUserDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required][MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}

