using System.ComponentModel.DataAnnotations;

namespace MedicalClinicAPI.DTOs.Admin;

public class CreateDoctorDTO
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required]
    public int SpecialtyId { get; set; }
    public string? PhoneNumber { get; set; }


}