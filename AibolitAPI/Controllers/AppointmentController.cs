using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAllAsync(
        [FromQuery] AppointmentFilterDTO? filterDto,
        int page = 1,
        int size = 10)
    {
        try
        {
            var appointments = await appointmentService.GetAllAsync(filterDto, page, size);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AppointmentDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var appointment = await appointmentService.GetByIdAsync(id);
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
            await appointmentService.CreateAsync(appointmentDto);
            return Created($"api/Appointment/{appointmentDto.Id}", appointmentDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] AppointmentDTO appointmentDto)
    {
        try
        {
            if (id != appointmentDto.Id) return BadRequest(new { message = "ID mismatch" });

            await appointmentService.UpdateAsync(appointmentDto);
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
            await appointmentService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}