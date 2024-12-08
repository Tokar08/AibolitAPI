using AibolitAPI.Interfaces;
using Google.Cloud.Translation.V2;

namespace AibolitAPI.Services;

public class DiseaseSearchService
{
    private readonly Func<string, IDiseaseSearchProvider> _providerFactory;
    private readonly ITranslationService _translationService;

    public DiseaseSearchService(Func<string, IDiseaseSearchProvider> providerFactory,
        ITranslationService translationService)
    {
        _providerFactory = providerFactory;
        _translationService = translationService;
    }

    public async Task<object> SearchAsync(string term, string provider)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new
            {
                Links = new List<string>(),
                Message = "Пожалуйста, укажите название болезни"
            };

        var searchProvider = _providerFactory(provider);

        if (provider.Equals("external", StringComparison.OrdinalIgnoreCase))
            term = await _translationService.TranslateAsync(term, LanguageCodes.English);

        return await searchProvider.SearchAsync(term);
    }
}