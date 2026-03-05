using System.ComponentModel.DataAnnotations;

namespace MedicalClinicAPI.DTOs.Appointments
{
    public class CreateAppointmentDTO
    {
        [Required]
        public int DoctorId { get; set; }

        public int? PatientId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }
    }
};