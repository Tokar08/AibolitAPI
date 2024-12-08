using AibolitAPI.Interfaces;

namespace AibolitAPI.Services;

public class NotificationService
{
    private readonly INotificationSender _emailSender;
    private readonly ILogger<NotificationService> _logger;
    private readonly IEmailTemplateFactory _templateFactory;

    public NotificationService(
        INotificationSender emailSender,
        IEmailTemplateFactory templateFactory,
        ILogger<NotificationService> logger)
    {
        _emailSender = emailSender;
        _templateFactory = templateFactory;
        _logger = logger;
    }

    public async Task SendEmailAsync(string recipient, string type, object model)
    {
        try
        {
            var template = _templateFactory.GetTemplate(type);
            await _emailSender.SendAsync(recipient, template, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending email notification.");
            throw;
        }
    }
}