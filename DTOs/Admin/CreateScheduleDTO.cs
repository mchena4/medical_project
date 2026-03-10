using System.ComponentModel.DataAnnotations;

namespace MedicalClinicAPI.DTOs.Admin;

public class CreateScheduleDTO
{
    [Required]
    public int DoctorId { get; set; }

    [Required]
    public int DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; } 

    [Required]
    public TimeSpan EndTime { get; set; }

    public int SlotDurationMinutes { get; set; } = 20; 
}