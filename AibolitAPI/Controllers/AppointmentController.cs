using AibolitAPI.DTOs;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var appointments = await _appointmentService.GetAllAsync(page, size);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();
            return Ok(appointment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] AppointmentDTO appointmentDto)
    {
        try
        {
            await _appointmentService.CreateAsync(appointmentDto);
            return Created($"api/Appointment/{appointmentDto.Id}", appointmentDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] AppointmentDTO appointmentDto)
    {
        try
        {
            if (id != appointmentDto.Id) return BadRequest(new { message = "ID mismatch" });

            await _appointmentService.UpdateAsync(appointmentDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> SoftDeleteAsync(Guid id)
    {
        try
        {
            await _appointmentService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}