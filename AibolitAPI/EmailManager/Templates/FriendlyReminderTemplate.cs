using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class FriendlyReminderTemplate : IEmailTemplate
{
    private readonly string _templatePath;

    public FriendlyReminderTemplate(string templatePath)
    {
        _templatePath = templatePath;
    }

    public string Subject => "Мы всегда рядом!";

    public string GetBody(object model)
    {
        var templateContent = File.ReadAllText(_templatePath);
        return templateContent;
    }
}