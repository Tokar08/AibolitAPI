using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager.Templates;

public class RegistrationSuccessTemplate : IEmailTemplate
{
    public string Subject => "Регистрация успешна";

    public string GetBody(object model)
    {
        return "<h1>Добро пожаловать!</h1><p>Вы успешно зарегистрировались в нашей системе.</p>";
    }
}