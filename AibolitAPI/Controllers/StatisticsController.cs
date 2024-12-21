using AibolitAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
//[AuthorizeRole("ChiefDoctor")]
public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStatistics(Guid id)
    {
        try
        {
            var statistics = await statisticsService.GetStatisticsAsync(id);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка при получении статистики: {ex.Message}");
        }
    }
}