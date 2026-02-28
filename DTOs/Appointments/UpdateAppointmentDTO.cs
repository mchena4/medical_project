using System.ComponentModel.DataAnnotations;

namespace MedicalClinicAPI.DTOs.Appointments
{
    public class UpdateAppointmentDTO
    {
        public DateTime? AppointmentDate { get; set; }
        public int? DoctorId { get; set; }

        public int? PatientId { get; set; }

        public int? StatusId { get; set; }
    }
}