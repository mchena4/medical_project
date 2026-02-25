using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;
using MedicalClinicAPI.DTOs.Auth;
using MedicalClinicAPI.Models;

namespace MedicalClinicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterDTO request)
    {
        // Check if the email is already registered
        if (await _context.Users.AnyAsync(u => u.Email == request.Email)){
            return BadRequest("Email is already registered.");
        }

        var patientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Patient");
        if (patientRole == null) 
        {
            return StatusCode(500, "Error: Role Patient not found in the database.");
        }

        // Create a new user
        var newUser = new User
        {
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = patientRole.Id,
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var newPatient = new Patient
        {
            UserId = newUser.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Dni = request.Dni,
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber
        };

        _context.Patients.Add(newPatient);
        await _context.SaveChangesAsync();

        return Ok("Patient registered successfully.");
    }
}