using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;
using MedicalClinicAPI.Models;
using MedicalClinicAPI.DTOs.Admin;
using MedicalClinicAPI.DTOs.Users;


namespace MedicalClinicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]

public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // This endpoint allows the admin to register new staff members (doctors and receptionists)
    [HttpPost("RegisterDoctor")]
    public async Task<IActionResult> RegisterDoctor(CreateDoctorDTO request)
    {
        // Check if the email is already in use
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Email is already in use.");
        }   

        // Validate the specialty ID
        var specialty = await _context.Specialties.FindAsync(request.SpecialtyId);
        if (specialty == null) return BadRequest("Invalid specialty ID.");

        // Find the role in the database
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Doctor");
        if (role == null) return StatusCode(500, "Role not found in the database.");

        // Create a new user 
        var newUser = new User
        {
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = role.Id
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        
        // Create a new doctor 
        var doctor = new Doctor
        {
            UserId = newUser.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            LicenseNumber = request.LicenseNumber,
            PhoneNumber = request.PhoneNumber,
            SpecialtyId = request.SpecialtyId
        };
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Doctor registered successfully" });
    }

    [HttpPost("RegisterReceptionist")]
    public async Task<IActionResult> RegisterReceptionist(CreateReceptionistDTO request)
    {
        // Check if the email is already in use
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("Email is already in use.");
        }

        // Find the role in the database
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Receptionist");
        if (role == null) return StatusCode(500, "Role not found in the database.");

        // Create a new user 
        var newUser = new User
        {
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = role.Id
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        
        // Create a new receptionist 
        var receptionist = new Receptionist
        {
            UserId = newUser.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };
        _context.Receptionists.Add(receptionist);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Receptionist registered successfully" });
    }

    [HttpPost("CreateSpecialty")]
    public async Task<IActionResult> CreateSpecialty(CreateSpecialtyDTO request)
    {
        // Check if the specialty already exists
        if (await _context.Specialties.AnyAsync(s => s.Name == request.Name))
        {
            return BadRequest("Specialty already exists.");
        }

        // Create a new specialty
        var specialty = new Specialty
        {
            Name = request.Name
        };

        _context.Specialties.Add(specialty);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Specialty created successfully" });
    }

    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser(CreateUserDTO request)
    {
        // Check if the email is already in use
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest( new{ message = "Email is already in use." });
        }

        // Find the role in the database
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
        if (role == null) return StatusCode(500, "Role not found in the database.");

        // Create a new user 
        var newUser = new User
        {
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = role.Id
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User created successfully" });
    }
}

