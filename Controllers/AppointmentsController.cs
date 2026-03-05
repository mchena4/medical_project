using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicAPI.Data;
using MedicalClinicAPI.Models;
using MedicalClinicAPI.DTOs.Appointments;
using MedicalClinicAPI.Extensions;

namespace MedicalClinicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = "Patient, Receptionist")]

    // This endpoint allows both patients and receptionists to create appointments.
    public async Task<IActionResult> CreateAppointment(CreateAppointmentDTO request)
    {
        //Identify the user and his role
        var (userId, userRole) = User.GetUserInfo();

        // Check if the user ID is valid
        if (userId == null) return Unauthorized("Invalid user ID.");

        // Patient id
        int finalPatientId = 0;

        // Get Patient id based on the user role
        if(userRole == "Patient")
        {
            // If the user is a patient, we get the patient id from the database using the user id
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
            {
                return NotFound( new { message = "Patient not found for the current user." });
            }

            // Use the id from database
            finalPatientId = patient.Id;
        }
        else if(userRole == "Receptionist")
        {
            // Check if patient id is getting provided
            if (request.PatientId == null)
            {
                return BadRequest( new{message = "PatientId is required for receptionists."});
            }

            // Use the id provided in the request
            finalPatientId = request.PatientId.Value;
        }

        // Check if the doctor exists
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId);
        if (doctor == null) return NotFound(new { message = "Doctor not found." });

        // Check if 'pending' status exists
        var pendingStatus = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == "Pending");
        if (pendingStatus == null) return StatusCode(500, new { message = "Error: Status Pending not found in the database." });
        
        // Create a new appointment
        var newAppointment = new Appointment
        {
            PatientId = finalPatientId,
            DoctorId = request.DoctorId,
            AppointmentDate = request.AppointmentDate,
            StatusId = pendingStatus.Id
        };

        _context.Appointments.Add(newAppointment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Appointment created successfully." });
    }

    [HttpGet]
    [Authorize(Roles = "Patient, Receptionist, Doctor")]

    // This endpoint allows patients, receptionists, and doctors to view appointments
    public async Task<IActionResult> GetAppointments()
    {
        //Identify the user and his role
        var (userId, userRole) = User.GetUserInfo();

        // Check if the user ID is valid
        if (userId == null) return Unauthorized("Invalid user ID.");

        // Query to get appointments with patient and status details
        var query = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Status)
            .AsQueryable();

        // Filter appointments based on user role
        if(userRole == "Patient")
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p=>p.UserId == userId);
            if(patient == null) return NotFound(new { message = "Patient not found for the current user." });

            // Only return appointments for the current patient
            query = query.Where(a=> a.PatientId == patient.Id);
        }

        if(userRole == "Doctor")
        {   
            // Only return appointments for the current doctor
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if(doctor == null) return NotFound(new { message = "Doctor not found for the current user." });
            query = query.Where(a => a.DoctorId == doctor.Id);
        }

        // Return the appointments with patient name and status
        var appointments = await query
            .OrderBy(a=>a.AppointmentDate)
            .Select(a => new
            {
                // Details of the appointment
                AppointmentId = a.Id,
                Date = a.AppointmentDate,
                Status = a.Status!.Name,
                Patient = a.Patient!.FirstName + " " + a.Patient.LastName,
                PatientDni = a.Patient!.Dni
            })
            .ToListAsync();

            return Ok(appointments);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Patient, Receptionist")]
    // This endpoint allows patients and receptionists to cancel appointments
    public async Task<IActionResult> CancelAppointment(int id)
    {
        //Identify the user and his role
        var (userId, userRole) = User.GetUserInfo();

        // Check if the user ID is valid
        if (userId == null) return Unauthorized(new { message = "Invalid user ID." });

        // Get the appointment to be cancelled
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound(new { message = "Appointment not found." });

        // If the user is a patient, check if the appointment belongs to the patient
        if (userRole == "Patient")
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p=>p.UserId == userId);

            if (patient == null || appointment.PatientId != patient.Id)
            {
                return StatusCode(403, new { message = "You are not authorized to cancel this appointment." });
            }

        }   

        // Cancel appointment
        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return Ok(new {message = "Appointment cancelled successfully." }); 
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Receptionist, Doctor")]

    // This endpoint allows receptionists and doctors to update appointments
    public async Task<IActionResult> UpdateAppointment(int id, UpdateAppointmentDTO request)
    {
        //Identify user and his role
        var (userId, userRole) = User.GetUserInfo();

        if (userId == null) return Unauthorized(new { message = "Invalid user ID." });


        // Get the appointment to be updated
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound(new { message = "Appointment not found." });

        
        if (userRole == "Doctor")
        {

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if(doctor == null) return NotFound(new { message = "Doctor not found for the current user." });
            
            // Check if the doctor is trying to update an appointment that belongs to him
            if(appointment.DoctorId != doctor.Id)
            {
                return StatusCode(403, new { message = "You are not authorized to update this appointment." });
            }

            // Only allow doctors to update the status of the appointment
            if (request.AppointmentDate.HasValue || request.DoctorId.HasValue || request.PatientId.HasValue)
            {
                return BadRequest(new { message = "Doctors can only update the status of the appointment." });
            }

            // Update the status if provided
            if(request.StatusId.HasValue) appointment.StatusId = request.StatusId.Value;

        }

        else if (userRole == "Receptionist")
        {
            // Check fields to update
            if (request.StatusId.HasValue)
            {
                appointment.StatusId = request.StatusId.Value;
            }

            if (request.AppointmentDate.HasValue)
            {
                appointment.AppointmentDate = request.AppointmentDate.Value;
            }

            if (request.DoctorId.HasValue)
            {
                // Check if the doctor exists and has the Doctor role
                var doctor = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.DoctorId.Value && u.Role != null && u.Role.Name == "Doctor");
                if (doctor == null) return NotFound(new { message = "Doctor not found." });
                appointment.DoctorId = request.DoctorId.Value;
            }
        }

        // Save changes to the database
        await _context.SaveChangesAsync();

        return Ok(new { message = "Appointment updated successfully." });
    }
}