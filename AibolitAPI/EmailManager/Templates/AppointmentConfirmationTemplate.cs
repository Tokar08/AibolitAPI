using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class AppointmentConfirmationTemplate : IEmailTemplate
{
    public string Subject => "Подтверждение записи на прием";

    public string GetBody(object model)
    {
        return "<h1>Ваш прием подтвержден!</h1><p>Детали будут отправлены позже.</p>";
    }
}