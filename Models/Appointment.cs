namespace MedicalClinicAPI.Models;

public class Appointment
{
    public int Id { get; set; }

    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }

    public int? DoctorId { get; set; }
    public User? Doctor { get; set; }

    public DateTime AppointmentDate { get; set; }
    
    public int? StatusId { get; set; }
    public Status? Status { get; set; }
}