namespace AibolitAPI.Interfaces;

public interface ITranslationService
{
    Task<string> TranslateAsync(string text, string targetLanguage);
}