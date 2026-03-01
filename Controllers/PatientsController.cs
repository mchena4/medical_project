using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;
using MedicalClinicAPI.DTOs.Patients;

namespace MedicalClinicalAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PatientsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Receptionist,Doctor")]

    // This endpoint allows receptionists and doctors to view all patients.
    public async Task<IActionResult> GetPatients()
    {
        var patients = await _context.Patients
            .Include(p => p.User)
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.Dni,
                p.DateOfBirth,
                p.PhoneNumber,
                UserEmail = p.User.Email
            })
            .ToListAsync();

            return Ok(patients);
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Receptionist")]
    // This endpoint allows receptionists to update patient information.
    public async Task<IActionResult> UpdatePatient(int id, UpdatePatientDTO request)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return NotFound("Patient not found.");

        // Update patient information 
        patient.FirstName = request.FirstName ?? patient.FirstName;
        patient.LastName = request.LastName ?? patient.LastName;
        patient.Dni = request.Dni ?? patient.Dni;
        patient.PhoneNumber = request.PhoneNumber ?? patient.PhoneNumber;
        patient.DateOfBirth = request.DateOfBirth ?? patient.DateOfBirth;
        await _context.SaveChangesAsync();
        return Ok("Patient updated successfully.");
    }
}