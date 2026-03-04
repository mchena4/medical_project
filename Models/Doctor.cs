namespace MedicalClinicAPI.Models;

public class Doctor
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
    public int SpecialtyId { get; set; }
    public Specialty? Specialty { get; set; }

}