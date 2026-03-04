using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;
using MedicalClinicAPI.Models;
using System.ComponentModel;

namespace MedicalClinicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class DoctorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DoctorsController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllDoctors()
    {
        // Fetch all doctors 
        var doctors = await _context.Doctors
            .Include(d => d.Specialty)
            .Select(d => new
            {
                Id = d.Id,
                Name = d.FirstName + " " + d.LastName,
                Specialty = d.Specialty.Name,
                PhoneNumber = d.PhoneNumber,
            })
            .ToListAsync();

        return Ok(doctors);
    }

    
    [HttpGet("Specialty/{SpecialtyId}")]
    // This endpoint retrieves doctors based on their specialty
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
                return NotFound("No doctors found for the specified specialty.");
            }  

            return Ok(doctors);
    }
}

