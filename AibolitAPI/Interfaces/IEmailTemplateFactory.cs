namespace AibolitAPI.Interfaces;

public interface IEmailTemplateFactory
{
    IEmailTemplate GetTemplate(string type);
}