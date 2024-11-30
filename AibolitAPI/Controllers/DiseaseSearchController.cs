using AibolitAPI.Interfaces;
using AibolitAPI.SearchProviders;
using AibolitAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiseaseSearchController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DiseaseSearchService _diseaseSearchService;

    public DiseaseSearchController(DiseaseSearchService diseaseSearchService, IConfiguration configuration)
    {
        _diseaseSearchService = diseaseSearchService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term, [FromQuery] string provider)
    {
        if (string.IsNullOrEmpty(provider) || (!provider.Equals("external", StringComparison.OrdinalIgnoreCase) &&
                                               !provider.Equals("openai", StringComparison.OrdinalIgnoreCase)))
            return BadRequest("Invalid provider specified.");

        IDiseaseSearchProvider searchProvider = provider.Equals("openai", StringComparison.OrdinalIgnoreCase)
            ? new OpenAIDiseaseSearchProvider()
            : new ExternalApiSearchProvider(new HttpClient());

        var result = await searchProvider.SearchAsync(term);

        return Ok(result);
    }
}