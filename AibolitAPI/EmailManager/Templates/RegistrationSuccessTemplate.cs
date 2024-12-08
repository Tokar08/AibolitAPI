using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class RegistrationSuccessTemplate : IEmailTemplate
{
    private readonly string _templatePath;

    public RegistrationSuccessTemplate(string templatePath)
    {
        _templatePath = templatePath;
    }

    public string Subject => "Welcome to our platform!";

    public string GetBody(object model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model is not string modelAsString)
            throw new InvalidCastException("Expected string but got something else.");

        var templateContent = File.ReadAllText(_templatePath);
        templateContent = templateContent.Replace("{UserName}", modelAsString);
        return templateContent;
    }
}