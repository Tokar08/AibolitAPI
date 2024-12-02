using AibolitAPI.Interfaces;
using Google.Cloud.Translation.V2;

namespace AibolitAPI.Services;

public class GoogleTranslationService : ITranslationService
{
    private readonly TranslationClient _translationClient;

    public GoogleTranslationService()
    {
        _translationClient = TranslationClient.Create();
    }

    public async Task<string> TranslateAsync(string text, string targetLanguage)
    {
        var response = await _translationClient.TranslateTextAsync(
            text,
            targetLanguage,
            LanguageCodes.Ukrainian
        );

        return response.TranslatedText;
    }
}