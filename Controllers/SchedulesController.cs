using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;
using MedicalClinicAPI.Models;
using MedicalClinicAPI.DTOs.Admin;

namespace MedicalClinicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class SchedulesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SchedulesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchedule(CreateScheduleDTO request)
    {
        var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId);
        if (!doctorExists) 
            return NotFound(new {message = "The specified doctor does not exist."});

        if (request.StartTime >= request.EndTime)
            return BadRequest(new {message = "The start time must be before the end time."});

        var scheduleExists = await _context.DoctorSchedules
            .AnyAsync(s => s.DoctorId == request.DoctorId && s.DayOfWeek == request.DayOfWeek);
            
        if (scheduleExists)
            return BadRequest(new{message = "Schedule already exists for this doctor on the specified day."});

        var newSchedule = new DoctorSchedule
        {
            DoctorId = request.DoctorId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDurationMinutes = request.SlotDurationMinutes
        };

        _context.DoctorSchedules.Add(newSchedule);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Schedule created successfully.", schedule = newSchedule });
    }
}