using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorController(
    IDoctorService doctorService,
    IRecommendationService recommendationService,
    IPrescriptionService prescriptionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var doctors = await doctorService.GetAllDoctorsWithSSOAsync(page, size);
            return Content(doctors, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DoctorDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var doctor = await doctorService.GetByIdAsync(id);
            return Content(doctor, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{doctorId:guid}/patients")]
    public async Task<IActionResult> GetPatientsForDoctor(Guid doctorId, int page = 1, int size = 10)
    {
        try
        {
            var patients = await doctorService.GetPatientsForDoctorAsync(doctorId, page, size);
            return Content(patients, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{doctorId:guid}/patients/{patientId:guid}")]
    public async Task<IActionResult> GetPatientById(Guid doctorId, Guid patientId)
    {
        try
        {
            var patient = await doctorService.GetPatientByIdAsync(doctorId, patientId);

            return Content(patient, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{doctorId:guid}/patients/{patientId:guid}/prescriptions")]
    public async Task<IActionResult> GetPrescriptionsForPatient(Guid doctorId, Guid patientId)
    {
        try
        {
            var prescriptions = await doctorService.GetPrescriptionsForPatientAsync(doctorId, patientId);
            return Content(prescriptions, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{doctorId:guid}/patients/{patientId:guid}/recommendations")]
    public async Task<IActionResult> GetRecommendationsForPatient(Guid doctorId, Guid patientId)
    {
        try
        {
            var recommendations = await doctorService.GetRecommendationsForPatientAsync(doctorId, patientId);

            return Content(recommendations, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{doctorId:guid}/patients/{patientId:guid}/prescriptions/{prescriptionId:guid}")]
    public async Task<IActionResult> GetPatientPrescriptionById(Guid doctorId, Guid patientId, Guid prescriptionId)
    {
        try
        {
            var prescription = await doctorService.GetPatientPrescriptionByIdAsync(doctorId, patientId, prescriptionId);

            return new JsonResult(prescription);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{doctorId:guid}/patients/{patientId:guid}/recommendations/{recommendationId:guid}")]
    public async Task<IActionResult> GetPatientRecommendationById(Guid doctorId, Guid patientId, Guid recommendationId)
    {
        try
        {
            var recommendation =
                await doctorService.GetPatientRecommendationByIdAsync(doctorId, patientId, recommendationId);

            return new JsonResult(recommendation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{doctorId:guid}/appointments")]
    public async Task<IActionResult> GetScheduledAppointmentsAsync(Guid doctorId)
    {
        var scheduledAppointments = await doctorService.GetScheduledAppointmentsByDoctorIdAsync(doctorId);

        if (!scheduledAppointments.Any())
            return NotFound("No scheduled appointments found for the specified doctor.");

        return Ok(scheduledAppointments);
    }

    [HttpDelete("{doctorId:guid}/appointments/{appointmentId:guid}/cancel")]
    public async Task<IActionResult> CancelAppointmentAsync(Guid doctorId, Guid appointmentId)
    {
        try
        {
            await doctorService.CancelAppointmentAsync(doctorId, appointmentId);
            return Ok("Appointment cancelled successfully.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] DoctorDTO doctorDto)
    {
        try
        {
            Stream? photoStream = null;

            var filePath = doctorDto.PhotoUrl;
            if (!string.IsNullOrWhiteSpace(doctorDto.PhotoUrl))
            {
                if (!System.IO.File.Exists(filePath))
                    return BadRequest(new { message = "File does not exist at the specified path." });

                photoStream = System.IO.File.OpenRead(filePath);
            }

            await doctorService.CreateWithPhotoAsync(doctorDto, photoStream,
                photoStream != null ? Path.GetFileName(filePath) : null);

            return Created($"api/Doctor/{doctorDto.Id}", new { message = "Doctor created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] DoctorDTO doctorDto)
    {
        try
        {
            if (id != doctorDto.Id)
                return BadRequest(new { message = "ID mismatch" });

            Stream? photoStream = null;
            var filePath = doctorDto.PhotoUrl;
            if (!string.IsNullOrWhiteSpace(doctorDto.PhotoUrl) && System.IO.File.Exists(doctorDto.PhotoUrl))
                photoStream = System.IO.File.OpenRead(filePath);

            await doctorService.UpdateWithPhotoAsync(id, doctorDto, photoStream,
                photoStream != null ? Path.GetFileName(filePath) : null);

            return Ok(new { message = "Doctor updated successfully", photoUrl = doctorDto.PhotoUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> SoftDeleteAsync(Guid id)
    {
        try
        {
            await doctorService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPost("{doctorId:guid}/patients/{patientId:guid}/prescriptions")]
    public async Task<IActionResult> CreatePrescriptionAsync(Guid doctorId, Guid patientId,
        [FromBody] PrescriptionDTO prescriptionDto)
    {
        try
        {
            await prescriptionService.CreateAsync(doctorId, patientId, prescriptionDto);
            return Created($"api/v1/Doctor/{doctorId}/patients/{patientId}/prescription/{prescriptionDto.Id}",
                prescriptionDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{doctorId:guid}/patients/{patientId:guid}/recommendations")]
    public async Task<IActionResult> CreateRecommendationAsync(Guid doctorId, Guid patientId,
        [FromBody] RecommendationDTO recommendationDto)
    {
        try
        {
            await recommendationService.CreateAsync(doctorId, patientId, recommendationDto);
            return Created($"api/v1/Doctor/{doctorId}/patients/{patientId}/recommendation/{recommendationDto.Id}",
                recommendationDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}