using AibolitAPI.DTOs;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionController : ControllerBase
{
    private readonly PrescriptionService _prescriptionService;

    public PrescriptionController(PrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PrescriptionDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var prescriptions = await _prescriptionService.GetAllAsync(page, size);
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PrescriptionDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound();
            return Ok(prescription);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] PrescriptionDTO prescriptionDto)
    {
        try
        {
            await _prescriptionService.CreateAsync(prescriptionDto);
            return Created($"api/Prescription/{prescriptionDto.Id}", prescriptionDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] PrescriptionDTO prescriptionDto)
    {
        try
        {
            if (id != prescriptionDto.Id) return BadRequest(new { message = "ID mismatch" });

            await _prescriptionService.UpdateAsync(prescriptionDto);
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
            await _prescriptionService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}