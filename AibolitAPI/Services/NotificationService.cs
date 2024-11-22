using AibolitAPI.DTOs;
using AibolitAPI.Interfaces;
using AibolitAPI.Models;
using AutoMapper;

namespace AibolitAPI.Services;

public class NotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository notificationRepository, IMapper mapper, ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<NotificationDTO>> GetAllAsync(int page, int size)
    {
        try
        {
            var notifications = await _notificationRepository.GetAllAsync(page, size);
            return _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all notifications.");
            throw;
        }
    }

    public async Task<NotificationDTO> GetByIdAsync(Guid id)
    {
        try
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            return _mapper.Map<NotificationDTO>(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting notification with ID: {id}");
            throw;
        }
    }

    public async Task<NotificationDTO> CreateAsync(NotificationDTO notificationDto)
    {
        try
        {
            var notification = _mapper.Map<Notification>(notificationDto);
            await _notificationRepository.CreateAsync(notification);
            return _mapper.Map<NotificationDTO>(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating notification.");
            throw;
        }
    }

    public async Task UpdateAsync(NotificationDTO notificationDto)
    {
        try
        {
            var notification = _mapper.Map<Notification>(notificationDto);
            await _notificationRepository.UpdateAsync(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating notification.");
            throw;
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        try
        {
            await _notificationRepository.SoftDeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while soft deleting notification with ID: {id}");
            throw;
        }
    }
}