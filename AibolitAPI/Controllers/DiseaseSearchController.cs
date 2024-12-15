using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiseaseSearchController(DiseaseSearchService diseaseSearchService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term, [FromQuery] string provider)
    {
        if (string.IsNullOrEmpty(provider) ||
            (!provider.Equals("external", StringComparison.OrdinalIgnoreCase) &&
             !provider.Equals("openai", StringComparison.OrdinalIgnoreCase)))
            return BadRequest("Invalid provider specified.");

        var result = await diseaseSearchService.SearchAsync(term, provider);

        return Ok(result);
    }
}