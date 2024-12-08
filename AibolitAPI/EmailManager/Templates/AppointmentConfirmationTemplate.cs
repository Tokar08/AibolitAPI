using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class AppointmentConfirmationTemplate : IEmailTemplate
{
    private readonly string _templatePath;

    public AppointmentConfirmationTemplate(string templatePath)
    {
        _templatePath = templatePath;
    }

    public string Subject => "Подтверждение записи на прием";

    public string GetBody(object model)
    {
        var templateContent = File.ReadAllText(_templatePath);
        return templateContent;
    }
}