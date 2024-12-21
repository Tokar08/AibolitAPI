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
        ArgumentNullException.ThrowIfNull(model);

        var templateContent = File.ReadAllText(_templatePath);

        if (model is object[] multipleValues)
            for (var i = 0; i < multipleValues.Length; i++)
                templateContent = templateContent.Replace($"{{{i}}}", multipleValues[i]?.ToString() ?? string.Empty);
        else
            throw new InvalidCastException($"Unsupported model type: {model.GetType()}");

        return templateContent;
    }
}