using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;

namespace MedicalClinicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class StatusesController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatusesController(AppDbContext context)
    {
        _context = context;
    }

    // This endpoint is for gettint all the appointment statuses in the frontend 
    [HttpGet]
    public async Task<IActionResult> GetStatuses()
    {
        var statuses = await _context.Statuses
            .Select(s=> new
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();
        return Ok(statuses);
    }
}