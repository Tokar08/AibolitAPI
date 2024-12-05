using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class NewPrescriptionTemplate : IEmailTemplate
{
    public string Subject => "Новый рецепт";

    public string GetBody(object model)
    {
        return "<h1>Новый рецепт!</h1><p>Ваш врач выписал новый рецепт. Пожалуйста, ознакомьтесь с деталями.</p>";
    }
}