using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HospitalController(IHospitalService hospitalService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HospitalDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var hospitals = await hospitalService.GetAllHospitalsWithDetailsAsync(page, size);
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
            var hospital = await hospitalService.GetByIdAsync(id);
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
            await hospitalService.CreateAsync(hospitalDto);
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

            await hospitalService.UpdateAsync(hospitalDto);
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
            await hospitalService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}