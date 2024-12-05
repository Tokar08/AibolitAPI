using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class FriendlyReminderTemplate : IEmailTemplate
{
    public string Subject => "Мы всегда рядом!";

    public string GetBody(object model)
    {
        return "<h1>Не болейте!</h1><p>Мы всегда рядом и ждем вас. Будьте здоровы и не забывайте о своем здоровье!</p>";
    }
}