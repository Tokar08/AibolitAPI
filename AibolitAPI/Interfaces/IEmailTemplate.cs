namespace AibolitAPI.Interfaces;

public interface IEmailTemplate
{
    string Subject { get; }
    string GetBody(object model);
}