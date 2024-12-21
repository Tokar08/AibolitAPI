namespace AibolitAPI.Interfaces.Services;

public interface IAppointmentReminderService
{
    Task SendRemindersAsync(CancellationToken cancellationToken);
}