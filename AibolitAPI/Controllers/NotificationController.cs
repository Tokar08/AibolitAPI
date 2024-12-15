using AibolitAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AibolitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController(NotificationService notificationService) : ControllerBase
{
    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromQuery] string recipient, [FromQuery] string type,
        [FromBody] string model)
    {
        try
        {
            await notificationService.SendEmailAsync(recipient, type, model);
            return Ok("Email sent successfully.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}