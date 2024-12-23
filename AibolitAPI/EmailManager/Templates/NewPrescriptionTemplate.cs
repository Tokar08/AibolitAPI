using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class NewPrescriptionTemplate(string templatePath) : IEmailTemplate
{
    public string Subject => "Новий рецепт!";

    public string GetBody(object model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var templateContent = File.ReadAllText(templatePath);

        if (model is object[] multipleValues)
            for (var i = 0; i < multipleValues.Length; i++)
                templateContent = templateContent.Replace($"{{{i}}}", multipleValues[i]?.ToString() ?? string.Empty);
        else
            throw new InvalidCastException($"Unsupported model type: {model.GetType()}");

        return templateContent;
    }
}