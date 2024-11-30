using System.Text.Json;
using AibolitAPI.Interfaces;

namespace AibolitAPI.SearchProviders;

public class ExternalApiSearchProvider : IDiseaseSearchProvider
{
    private readonly HttpClient _httpClient;

    public ExternalApiSearchProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<object> SearchAsync(string term)
    {
        var apiUrl =
            $"https://lforms-service-vip.nlm.nih.gov/api/conditions/v3/search?sf=info_link_data&df=info_link_data&terms={term}";
        var response = await _httpClient.GetAsync(apiUrl);

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

        var links = ExtractUniqueLinks(linksElement);

        return new
        {
            Links = links,
            Message = links.Count > 0 ? "Results found" : "No results found. Please check your input"
        };
    }

    private static List<string> ExtractUniqueLinks(JsonElement linksElement)
    {
        var links = new HashSet<string>();

        foreach (var item in linksElement.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Array)
                continue;

            var firstElement = item.EnumerateArray().FirstOrDefault();

            if (firstElement.ValueKind != JsonValueKind.String)
                continue;

            var link = firstElement.GetString()?.Split(',').FirstOrDefault();

            if (!string.IsNullOrEmpty(link)) links.Add(link);
        }

        return links.ToList();
    }
}