using AibolitAPI.DTOs;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly PatientService _patientService;

    public PatientController(PatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var patients = await _patientService.GetAllAsync(page, size);
            return Ok(patients);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null)
                return NotFound();
            return Ok(patient);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] PatientDTO patientDto)
    {
        try
        {
            await _patientService.CreateAsync(patientDto);
            return Created($"api/Patient/{patientDto.Id}", patientDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] PatientDTO patientDto)
    {
        try
        {
            if (id != patientDto.Id) return BadRequest(new { message = "ID mismatch" });

            await _patientService.UpdateAsync(patientDto);
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
            await _patientService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}