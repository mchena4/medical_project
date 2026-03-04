namespace MedicalClinicAPI.Models;

public class Receptionist
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}
    