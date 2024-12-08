using System.Text.Json;
using AibolitAPI.Interfaces;

namespace AibolitAPI.SearchProviders;

public class ExternalApiSearchProvider : IDiseaseSearchProvider
{
    private readonly string _apiUrl;
    private readonly HttpClient _httpClient;

    public ExternalApiSearchProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiUrl = configuration["ApiSettings:ExternalApiUrl"];
    }

    public async Task<object> SearchAsync(string term)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}{term}");

        if (!response.IsSuccessStatusCode)
            return new
            {
                Links = new List<string>(),
                Message = "Error accessing the external API"
            };

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return ParseApiResponse(jsonResponse);
    }

    private static object ParseApiResponse(string jsonResponse)
    {
        var parsedResponse = JsonSerializer.Deserialize<List<object>>(jsonResponse);

        if (parsedResponse == null || parsedResponse.Count < 4 || parsedResponse[3] is not JsonElement linksElement)
            return new
            {
                Links = new List<string>(),
                Message = "No results found. Please check your input"
            };

        var links = linksElement.EnumerateArray()
            .Where(e => e.ValueKind == JsonValueKind.Array)
            .Select(e => e.EnumerateArray().FirstOrDefault().GetString())
            .Where(link => !string.IsNullOrEmpty(link))
            .Distinct()
            .ToList();

        return new
        {
            Links = links,
            Message = links.Count > 0 ? "Results found" : "No results found. Please check your input"
        };
    }
}