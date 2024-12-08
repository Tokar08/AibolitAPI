namespace AibolitAPI.Interfaces;

public interface INotificationSender
{
    Task SendAsync(string recipient, IEmailTemplate template, object model);
}