using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class DiseaseSearchInfoTemplate(string templatePath) : IEmailTemplate
{
    public string Subject => "Нагадування про пошук хвороб!";

    public string GetBody(object model)
    {
        var templateContent = File.ReadAllText(templatePath);
        return templateContent;
    }
}