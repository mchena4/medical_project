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

namespace MedicalClinicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDTO request)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };  

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings.GetValue<string>("SecretKey");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
        (
            issuer: jwtSettings.GetValue<string>("Issuer"),
            audience: jwtSettings.GetValue<string>("Audience"),
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials 

        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { Token = tokenString });

    }
}