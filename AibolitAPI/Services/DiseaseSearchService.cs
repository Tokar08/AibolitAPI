using AibolitAPI.Interfaces;
using Google.Cloud.Translation.V2;

namespace AibolitAPI.Services;

public class DiseaseSearchService
{
    private readonly IDiseaseSearchProvider _searchProvider;
    private readonly ITranslationService _translationService;

    public DiseaseSearchService(IDiseaseSearchProvider searchProvider, ITranslationService translationService)
    {
        _searchProvider = searchProvider;
        _translationService = translationService;
    }

    public async Task<object> SearchAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new
            {
                Links = new List<string>(),
                Message = "Пожалуйста, укажите название болезни"
            };

        var translatedTerm = await _translationService.TranslateAsync(term, LanguageCodes.English);
        return await _searchProvider.SearchAsync(translatedTerm);
    }
}