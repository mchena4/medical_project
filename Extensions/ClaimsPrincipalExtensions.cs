using System.Security.Claims;

namespace MedicalClinicAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static (int? userId, string? role) GetUserInfo(this ClaimsPrincipal user)
        {
            // Identify user and his role
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the user ID is valid and return it along with the role
            if (int.TryParse(userIdString, out int userId))
            {
                return (userId, role);
            }

            // If conversion fails, return null and role
            return (null, role);
        }
    }
}