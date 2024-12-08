using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class NewPrescriptionTemplate : IEmailTemplate
{
    private readonly string _templatePath;

    public NewPrescriptionTemplate(string templatePath)
    {
        _templatePath = templatePath;
    }

    public string Subject => "Новый рецепт";

    public string GetBody(object model)
    {
        var templateContent = File.ReadAllText(_templatePath);
        return templateContent;
    }
}