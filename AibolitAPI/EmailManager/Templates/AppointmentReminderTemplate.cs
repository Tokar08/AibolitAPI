using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class AppointmentReminderTemplate : IEmailTemplate
{
    private readonly string _templatePath;

    public AppointmentReminderTemplate(string templatePath)
    {
        _templatePath = templatePath;
    }

    public string Subject => "Напоминание о приеме";

    public string GetBody(object model)
    {
        var templateContent = File.ReadAllText(_templatePath);
        return templateContent;
    }
}