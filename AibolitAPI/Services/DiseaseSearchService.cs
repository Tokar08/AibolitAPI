using AibolitAPI.Interfaces;

namespace AibolitAPI.Services;

public class DiseaseSearchService
{
    private readonly IDiseaseSearchProvider _searchProvider;

    public DiseaseSearchService(IDiseaseSearchProvider searchProvider)
    {
        _searchProvider = searchProvider;
    }

    public async Task<object> SearchAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new
            {
                Links = new List<string>(),
                Message = "Пожалуйста, укажите название болезни."
            };

        return await _searchProvider.SearchAsync(term);
    }
}