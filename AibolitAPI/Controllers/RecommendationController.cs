using AibolitAPI.DTOs;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecommendationController : ControllerBase
{
    private readonly RecommendationService _recommendationService;

    public RecommendationController(RecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecommendationDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var recommendations = await _recommendationService.GetAllAsync(page, size);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecommendationDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var recommendation = await _recommendationService.GetByIdAsync(id);
            if (recommendation == null)
                return NotFound();
            return Ok(recommendation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] RecommendationDTO recommendationDto)
    {
        try
        {
            await _recommendationService.CreateAsync(recommendationDto);
            return Created($"api/Recommendation/{recommendationDto.Id}", recommendationDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] RecommendationDTO recommendationDto)
    {
        try
        {
            if (id != recommendationDto.Id) return BadRequest(new { message = "ID mismatch" });

            await _recommendationService.UpdateAsync(recommendationDto);
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
            await _recommendationService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}