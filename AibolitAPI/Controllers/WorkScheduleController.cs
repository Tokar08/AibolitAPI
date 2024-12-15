using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkScheduleController(IWorkScheduleService workScheduleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkScheduleDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var workSchedules = await workScheduleService.GetAllAsync(page, size);
            return Ok(workSchedules);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkScheduleDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var workSchedule = await workScheduleService.GetByIdAsync(id);
            if (workSchedule == null)
                return NotFound();
            return Ok(workSchedule);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] WorkScheduleDTO workScheduleDto)
    {
        try
        {
            await workScheduleService.CreateAsync(workScheduleDto);
            return Created($"api/WorkSchedule/{workScheduleDto.Id}", workScheduleDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] WorkScheduleDTO workScheduleDto)
    {
        try
        {
            if (id != workScheduleDto.Id) return BadRequest(new { message = "ID mismatch" });

            await workScheduleService.UpdateAsync(workScheduleDto);
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
            await workScheduleService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}