using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromQuery] string recipient, [FromQuery] string type)
    {
        try
        {
            await _notificationService.SendEmailAsync(recipient, type);
            return Ok("Email sent successfully.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}