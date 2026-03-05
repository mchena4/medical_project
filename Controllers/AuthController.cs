using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;
using MedicalClinicAPI.DTOs.Auth;
using MedicalClinicAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using MedicalClinicAPI.Extensions;

namespace MedicalClinicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    // Constructor to inject the database context and configuration
    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Register endpoint to create a new patient user
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterDTO request)
    {
        // Check if the email is already registered
        if (await _context.Users.AnyAsync(u => u.Email == request.Email)){
            return BadRequest(new{message = "Email is already registered."});
        }

        // Get the Patient role from the database
        var patientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Patient");
        if (patientRole == null) 
        {
            return StatusCode(500, new{message = "Error: Role Patient not found in the database."});
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

        // Create a new patient linked to the user
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

        return Ok(new{message = "Patient registered successfully."});
    }

    // Login endpoint to authenticate a user and return a JWT token
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDTO request)
    {
        // Find the user by email and include the role information
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // Check if the user exists and if the password is correct
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Unauthorized(new{message = "Invalid email or password."});
        }

        // Create claims for the JWT token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "Patient")
        };  

        // Generate the JWT token
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings.GetValue<string>("SecretKey");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
        (
            issuer: jwtSettings.GetValue<string>("Issuer"),
            audience: jwtSettings.GetValue<string>("Audience"),
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials 

        );

        // Return the token as a string
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { Token = tokenString });

    }

    [HttpPost("ChangePassword")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDTO request)
    {
        var userId = User.GetUserInfo().userId;
        if (userId == null) return Unauthorized(new{message = "User ID not found in token."});

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound(new{message = "User not found."});

        if(!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
        {
            return BadRequest(new{message = "Current password is incorrect."});
        }

        if (request.CurrentPassword == request.NewPassword)
        {
            return BadRequest(new{message = "New password cannot be the same as the current password."});
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new{message = "Password changed successfully."});
    }
}