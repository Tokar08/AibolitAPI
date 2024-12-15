using AibolitAPI.Attributes;
using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdministratorController(IAdministratorService administratorService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdministratorDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var administrators = await administratorService.GetAllAdministratorWithSSOAsync(page, size);
            return Ok(administrators);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdministratorDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var administrator = await administratorService.GetByIdAsync(id);
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
            await administratorService.CreateAsync(administratorDto);
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

            await administratorService.UpdateAsync(administratorDto);
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
            await administratorService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}