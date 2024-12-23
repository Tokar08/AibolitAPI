using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class FriendlyReminderTemplate(string templatePath) : IEmailTemplate
{
    public string Subject => "Ми завжди поряд!";

    public string GetBody(object model)
    {
        var templateContent = File.ReadAllText(templatePath);
        return templateContent;
    }
}