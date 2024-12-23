using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientController(IPatientService patientService) : ControllerBase
{
    [HttpGet("{patientId:guid}/appointments")]
    public async Task<IActionResult> GetAllAppointmentsAsync(Guid patientId, int page = 1, int size = 10)
    {
        try
        {
            var appointmentsJson = await patientService.GetAllAppointmentsAsync(patientId, page, size);
            return Content(appointmentsJson, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{doctorId:guid}/appointments")]
    public async Task<IActionResult> CreateAppointment(Guid doctorId, Guid patientId,
        [FromQuery] DateTime appointmentDate)
    {
        try
        {
            var ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
            var appointmentDateUtc = TimeZoneInfo.ConvertTimeToUtc(appointmentDate, ukraineTimeZone);

            await patientService.CreateAppointmentAsync(doctorId, patientId, appointmentDateUtc);
            Log.Information(
                $"Creating appointment: DoctorId={doctorId}, PatientId={patientId}, AppointmentDate={appointmentDateUtc}");
            return Ok(new { message = "Appointment successfully created." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpDelete("{patientId:guid}/appointments/{appointmentId:guid}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid patientId, Guid appointmentId)
    {
        try
        {
            await patientService.CancelAppointmentAsync(patientId, appointmentId);
            return Ok(new { message = "Appointment successfully canceled." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("doctors")]
    public async Task<IActionResult> GetDoctorsAsync(int page = 1, int size = 10)
    {
        try
        {
            var doctors = await patientService.GetAllDoctorsAsyncWithSSO(page, size);
            return Content(doctors, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavoriteDoctors(Guid patientId, int page = 1, int size = 10)
    {
        try
        {
            var favoriteDoctors = await patientService.GetFavoriteDoctorsWithSSOAsync(patientId, page, size);
            return Content(favoriteDoctors, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{patientId:guid}/favorite/{doctorId:guid}")]
    public async Task<IActionResult> AddDoctorToFavorites(Guid patientId, Guid doctorId)
    {
        try
        {
            await patientService.AddDoctorToFavoritesAsync(patientId, doctorId);
            return Ok(new { message = "Doctor added to favorites successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("favorites/{doctorId:guid}")]
    public async Task<IActionResult> RemoveDoctorFromFavoritesAsync(Guid patientId, Guid doctorId)
    {
        try
        {
            await patientService.RemoveDoctorFromFavoritesAsync(patientId, doctorId);
            return Ok(new { message = "Doctor removed from favorites successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{patientId:guid}/prescriptions")]
    public async Task<IActionResult> GetPrescriptions(Guid patientId)
    {
        try
        {
            var prescriptions = await patientService.GetPrescriptionsForPatientAsync(patientId);
            return Content(prescriptions, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{patientId:guid}/recommendations")]
    public async Task<IActionResult> GetRecommendations(Guid patientId)
    {
        try
        {
            var recommendations = await patientService.GetRecommendationsForPatientAsync(patientId);
            return Content(recommendations, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{patientId:guid}/prescriptions/{prescriptionId:guid}")]
    public async Task<IActionResult> GetPrescriptionById(Guid patientId, Guid prescriptionId)
    {
        try
        {
            var prescription = await patientService.GetPrescriptionByIdWithDoctorAsync(patientId, prescriptionId);
            return new JsonResult(prescription);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpGet("{patientId:guid}/recommendations/{recommendationId:guid}")]
    public async Task<IActionResult> GetRecommendationById(Guid patientId, Guid recommendationId)
    {
        try
        {
            var recommendation = await patientService.GetRecommendationByIdWithDoctorAsync(patientId, recommendationId);
            return new JsonResult(recommendation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var patients = await patientService.GetAllPatientsWithSSOAsync(page, size);
            return new ContentResult
            {
                Content = patients,
                ContentType = "application/json"
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var patient = await patientService.GetByIdAsync(id);
            return new ContentResult
            {
                Content = patient,
                ContentType = "application/json"
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] PatientDTO patientDto)
    {
        try
        {
            await patientService.CreateAsync(patientDto);
            return Created($"api/Patient/{patientDto.Id}", patientDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] PatientDTO patientDto)
    {
        try
        {
            if (id != patientDto.Id) return BadRequest(new { message = "ID mismatch" });

            await patientService.UpdateAsync(patientDto);
            return NoContent();
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
            await patientService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}