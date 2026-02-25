namespace MedicalClinicAPI.Models;

public class Patient {
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; } 

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string Dni { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}