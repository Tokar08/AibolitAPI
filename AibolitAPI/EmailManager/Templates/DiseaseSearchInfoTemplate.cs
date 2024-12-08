using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class DiseaseSearchInfoTemplate : IEmailTemplate
{
    private readonly string _templatePath;

    public DiseaseSearchInfoTemplate(string templatePath)
    {
        _templatePath = templatePath;
    }

    public string Subject => "О поиске болезней";

    public string GetBody(object model)
    {
        var templateContent = File.ReadAllText(_templatePath);
        return templateContent;
    }
}