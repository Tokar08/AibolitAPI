using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class NewRecommendationTemplate : IEmailTemplate
{
    public string Subject => "Новая рекомендация";

    public string GetBody(object model)
    {
        return
            "<h1>У вас новая рекомендация!</h1><p>Пожалуйста, ознакомьтесь с рекомендациями, предоставленными вашим врачом.</p>";
    }
}