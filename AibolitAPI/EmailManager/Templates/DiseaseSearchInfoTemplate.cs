using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class DiseaseSearchInfoTemplate : IEmailTemplate
{
    public string Subject => "О поиске болезней";

    public string GetBody(object model)
    {
        return
            "<h1>Мы помогаем находить заболевания!</h1><p>Наш сервис помогает находить информацию о заболеваниях. Используйте его для поиска нужной информации.</p>";
    }
}