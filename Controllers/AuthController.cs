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
            return StatusCode(500, "Error interno: El rol 'Patient' no existe en la base de datos.");
        }

        // Create a new user
        var user = new User
        {
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = patientRole.Id,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }
}