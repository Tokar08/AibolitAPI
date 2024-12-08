using AibolitAPI.Attributes;
using AibolitAPI.DTOs;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdministratorController : ControllerBase
{
    private readonly AdministratorService _administratorService;

    public AdministratorController(AdministratorService administratorService)
    {
        _administratorService = administratorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdministratorDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var administrators = await _administratorService.GetAllAsync(page, size);
            return Ok(administrators);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [AuthorizeRole("Patient")]
    public async Task<ActionResult<AdministratorDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var administrator = await _administratorService.GetByIdAsync(id);
            if (administrator == null)
                return NotFound();
            return Ok(administrator);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] AdministratorDTO administratorDto)
    {
        try
        {
            await _administratorService.CreateAsync(administratorDto);
            return Created($"api/Administrator/{administratorDto.Id}", administratorDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] AdministratorDTO administratorDto)
    {
        try
        {
            if (id != administratorDto.Id) return BadRequest(new { message = "ID mismatch" });

            await _administratorService.UpdateAsync(administratorDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [AuthorizeRole("Admin")]
    public async Task<ActionResult> SoftDeleteAsync(Guid id)
    {
        try
        {
            await _administratorService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}