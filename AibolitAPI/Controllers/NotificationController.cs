using AibolitAPI.DTOs;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetAllAsync(int page = 1, int size = 10)
    {
        try
        {
            var notifications = await _notificationService.GetAllAsync(page, size);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            var notification = await _notificationService.GetByIdAsync(id);
            if (notification == null)
                return NotFound();
            return Ok(notification);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromBody] NotificationDTO notificationDto)
    {
        try
        {
            var createdNotification = await _notificationService.CreateAsync(notificationDto);
            return Created($"api/Notification/{createdNotification.Id}", createdNotification);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] NotificationDTO notificationDto)
    {
        if (id != notificationDto.Id)
            return BadRequest(new { message = "ID mismatch" });

        try
        {
            await _notificationService.UpdateAsync(notificationDto);
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
            await _notificationService.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}