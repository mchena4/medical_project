using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.OutputCaching;
using MedicalClinicAPI.Filters;

namespace MedicalClinicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting("BasicPolitics")]
public class DoctorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DoctorsController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    [OutputCache(PolicyName = "DoctorsPolicy")]
    public async Task<IActionResult> GetAllDoctors()
    {
        // Fetch all doctors 
        var doctors = await _context.Doctors
            .Include(d => d.Specialty)
            .Select(d => new
            {
                Id = d.Id,
                Name = d.FirstName + " " + d.LastName,
                LicenseNumber = d.LicenseNumber,
                Specialty = d.Specialty!.Name,
                PhoneNumber = d.PhoneNumber,
            })
            .ToListAsync();

        return Ok(doctors);
    }

    
    [HttpGet("Specialty/{SpecialtyId}")]
    // This endpoint retrieves doctors based on their specialty
    [OutputCache(PolicyName = "DoctorsPolicy", VaryByRouteValueNames = new[] { "SpecialtyId"})]
    public async Task<IActionResult> GetDoctorsBySpecialty(int SpecialtyId)
    {
        // Fetch doctors by specialty
        var doctors = await _context.Doctors
            .Include(d => d.Specialty)
            .Where(d => d.SpecialtyId == SpecialtyId)
            .Select(d => new
            {
                Id = d.Id,
                Name = d.FirstName + " " + d.LastName,
                LicenseNumber = d.LicenseNumber
            })
            .ToListAsync();

            // Check if any doctors were found for the specified specialty
            if(doctors.Count == 0)
            {
                return NotFound(new { message = "No doctors found for the specified specialty." });
            }  

            return Ok(doctors);
    }
}

