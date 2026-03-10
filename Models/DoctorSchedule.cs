using System.Text.Json.Serialization;

namespace MedicalClinicAPI.Models;

public class DoctorSchedule
{
    public int Id { get; set; }
    
    public int DoctorId { get; set; }
    
    [JsonIgnore] 
    public Doctor? Doctor { get; set; }

    public int DayOfWeek { get; set; } 

    public TimeSpan StartTime { get; set; } 

    public TimeSpan EndTime { get; set; }   

    public int SlotDurationMinutes { get; set; } = 20; 
}