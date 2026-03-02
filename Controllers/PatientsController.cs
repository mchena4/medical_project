using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;
using MedicalClinicAPI.DTOs.Patients;
using MedicalClinicAPI.Models;

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

    [HttpPost]
    [Authorize(Roles = "Receptionist")]
    // This endpoint allows receptionists to create new patients.
    public async Task<IActionResult> CreatePatient(CreatePatientDTO request)
    {
        // Get the patient role from the database
        var patientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Patient");
        if (patientRole == null) return StatusCode(500, "Patient role not found in the database.");

        // Check if a patient with the same DNI or email already exists
        if(await _context.Patients.AnyAsync(p => p.Dni == request.Dni))
        {
            return BadRequest("A patient with the same DNI already exists.");
        }

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("This email is already in use.");
        }

        // Create a new user for the patient
        var newUser = new User 
        {
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Dni), // Using DNI as the initial password, but it should be changed by the user later.
            RoleId = patientRole.Id
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();


        // Create a new patient linked to the user
        var patient = new Patient
        {
            UserId = newUser.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Dni = request.Dni,
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber ?? "N/A",        
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return Ok("Patient created successfully.");
    }
}