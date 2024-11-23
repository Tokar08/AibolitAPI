using AibolitAPI.DTOs;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HospitalController : ControllerBase
{
    private readonly HospitalService _hospitalService;

    public HospitalController(HospitalService hospitalService)
    {
        _hospitalService = hospitalService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HospitalDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var hospitals = await _hospitalService.GetAllAsync(page, size);
            return Ok(hospitals);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HospitalDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var hospital = await _hospitalService.GetByIdAsync(id);
            if (hospital == null)
                return NotFound();
            return Ok(hospital);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] HospitalDTO hospitalDto)
    {
        try
        {
            await _hospitalService.CreateAsync(hospitalDto);
            return Created($"api/Hospital/{hospitalDto.Id}", hospitalDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] HospitalDTO hospitalDto)
    {
        try
        {
            if (id != hospitalDto.Id) return BadRequest(new { message = "ID mismatch" });

            await _hospitalService.UpdateAsync(hospitalDto);
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
            await _hospitalService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}