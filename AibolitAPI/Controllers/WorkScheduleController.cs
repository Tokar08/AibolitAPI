using AibolitAPI.DTOs;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkScheduleController : ControllerBase
{
    private readonly WorkScheduleService _workScheduleService;

    public WorkScheduleController(WorkScheduleService workScheduleService)
    {
        _workScheduleService = workScheduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkScheduleDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var workSchedules = await _workScheduleService.GetAllAsync(page, size);
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
            var workSchedule = await _workScheduleService.GetByIdAsync(id);
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
            await _workScheduleService.CreateAsync(workScheduleDto);
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

            await _workScheduleService.UpdateAsync(workScheduleDto);
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
            await _workScheduleService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}