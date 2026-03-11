using System.ComponentModel.DataAnnotations;

namespace MedicalClinicAPI.DTOs.Appointments
{
    public class CreateAppointmentDTO
    {
        [Required]
        public int DoctorId { get; set; }

        public int? PatientId { get; set; }

        [Required]
        private DateTime _appointmentDate;

        public DateTime AppointmentDate
        {
            get => _appointmentDate;
            set => _appointmentDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);        
        }
    }
};