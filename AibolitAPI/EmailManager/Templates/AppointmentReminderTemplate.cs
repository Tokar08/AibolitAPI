using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class AppointmentReminderTemplate : IEmailTemplate
{
    public string Subject => "Напоминание о приеме";

    public string GetBody(object model)
    {
        return "<h1>Напоминание!</h1><p>Не забудьте о своем приеме!</p>";
    }
}