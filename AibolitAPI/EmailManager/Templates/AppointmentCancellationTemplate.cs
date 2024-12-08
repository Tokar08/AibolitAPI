using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class AppointmentCancellationTemplate : IEmailTemplate
{
    private readonly string _templatePath;

    public AppointmentCancellationTemplate(string templatePath)
    {
        _templatePath = templatePath;
    }

    public string Subject => "Отмена записи на прием";

    public string GetBody(object model)
    {
        var templateContent = File.ReadAllText(_templatePath);
        return templateContent;
    }
}