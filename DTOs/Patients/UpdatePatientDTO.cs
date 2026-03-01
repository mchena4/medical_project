namespace MedicalClinicAPI.DTOs.Patients
{
    public class UpdatePatientDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Dni { get; set; }

        public string? PhoneNumber { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}