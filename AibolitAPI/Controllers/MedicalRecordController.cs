using AibolitAPI.DTOs;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicalRecordController : ControllerBase
{
    private readonly MedicalRecordService _medicalRecordService;

    public MedicalRecordController(MedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicalRecordDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var medicalRecords = await _medicalRecordService.GetAllAsync(page, size);
            return Ok(medicalRecords);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MedicalRecordDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var medicalRecord = await _medicalRecordService.GetByIdAsync(id);
            if (medicalRecord == null)
                return NotFound();
            return Ok(medicalRecord);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] MedicalRecordDTO medicalRecordDto)
    {
        try
        {
            await _medicalRecordService.CreateAsync(medicalRecordDto);
            return Created($"api/MedicalRecord/{medicalRecordDto.Id}", medicalRecordDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] MedicalRecordDTO medicalRecordDto)
    {
        try
        {
            if (id != medicalRecordDto.Id) return BadRequest(new { message = "ID mismatch" });

            await _medicalRecordService.UpdateAsync(medicalRecordDto);
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
            await _medicalRecordService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}