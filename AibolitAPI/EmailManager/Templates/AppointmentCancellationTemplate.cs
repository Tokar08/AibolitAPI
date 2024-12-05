using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class AppointmentCancellationTemplate : IEmailTemplate
{
    public string Subject => "Отмена записи на прием";

    public string GetBody(object model)
    {
        return "<h1>Ваш прием отменен!</h1><p>Мы сожалеем, но ваша запись на прием была отменена.</p>";
    }
}