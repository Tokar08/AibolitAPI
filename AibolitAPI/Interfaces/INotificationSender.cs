namespace AibolitAPI.Interfaces;

public interface INotificationSender
{
    Task SendAsync(string recipient, object model);
}