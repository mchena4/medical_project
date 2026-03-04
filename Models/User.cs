using System.Text.Json.Serialization;

namespace MedicalClinicAPI.Models;

public class User {
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public string Password { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}