using AibolitAPI.DTOs;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorController : ControllerBase
{
    private readonly DoctorService _doctorService;

    public DoctorController(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var doctors = await _doctorService.GetAllAsync(page, size);
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] DoctorDTO doctorDto)
    {
        try
        {
            Stream? photoStream = null;

            var filePath = doctorDto.PhotoUrl;
            if (!string.IsNullOrWhiteSpace(doctorDto.PhotoUrl))
            {
                if (!System.IO.File.Exists(filePath))
                    return BadRequest(new { message = "File does not exist at the specified path." });

                photoStream = System.IO.File.OpenRead(filePath);
            }

            await _doctorService.CreateWithPhotoAsync(doctorDto, photoStream,
                photoStream != null ? Path.GetFileName(filePath) : null);

            return Created($"api/Doctor/{doctorDto.Id}", new { message = "Doctor created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] DoctorDTO doctorDto)
    {
        try
        {
            if (id != doctorDto.Id)
                return BadRequest(new { message = "ID mismatch" });

            Stream? photoStream = null;
            var filePath = doctorDto.PhotoUrl;
            if (!string.IsNullOrWhiteSpace(doctorDto.PhotoUrl) && System.IO.File.Exists(doctorDto.PhotoUrl))
                photoStream = System.IO.File.OpenRead(filePath);

            await _doctorService.UpdateWithPhotoAsync(id, doctorDto, photoStream,
                photoStream != null ? Path.GetFileName(filePath) : null);

            return Ok(new { message = "Doctor updated successfully", photoUrl = doctorDto.PhotoUrl });
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
            await _doctorService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}