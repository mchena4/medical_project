using System.Text.Json.Serialization;

namespace MedicalClinicAPI.Models;

public class Specialty
{   
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property to Doctors
    [JsonIgnore]
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

}