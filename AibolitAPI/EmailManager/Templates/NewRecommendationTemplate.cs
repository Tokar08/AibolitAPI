using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class NewRecommendationTemplate : IEmailTemplate
{
    private readonly string _templatePath;

    public NewRecommendationTemplate(string templatePath)
    {
        _templatePath = templatePath;
    }

    public string Subject => "Новая рекомендация";

    public string GetBody(object model)
    {
        var templateContent = File.ReadAllText(_templatePath);
        return templateContent;
    }
}