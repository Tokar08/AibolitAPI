using System.Text;
using AibolitAPI.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GeminiDiseaseSearchProvider : IDiseaseSearchProvider
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public GeminiDiseaseSearchProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<object> SearchAsync(string term)
    {
        var baseUrl = _configuration["ApiSettings:GeminiBaseUrl"];
        var apiKey = _configuration["ApiSettings:GeminiKey"];
        var prompt = _configuration["ApiSettings:GeminiPrompt"];
        var fullPrompt = $"{prompt}\n{term}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[] { new { text = fullPrompt } }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 1024
            }
        };

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(requestBody, Formatting.Indented),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{baseUrl}?key={apiKey}", jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return new
            {
                Message = $"Error: {response.StatusCode}, Details: {errorContent}"
            };
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JObject.Parse(responseContent);

        var text = apiResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

        if (string.IsNullOrEmpty(text)) return new { Message = "No content found" };

        var structuredResponse = FormatTextToStructuredFormat(text);

        return new
        {
            Message = "Results fetched successfully",
            Content = structuredResponse
        };
    }

    private object FormatTextToStructuredFormat(string text)
    {
        var result = new List<object>();

        var paragraphs = text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        foreach (var paragraph in paragraphs)
            if (paragraph.Contains("важливо"))
                result.Add(new
                {
                    Type = "Reminder",
                    Text = paragraph
                });
            else if (paragraph.Contains("Загальні фактори"))
                result.Add(new
                {
                    Type = "Heading",
                    Text = paragraph
                });
            else if (paragraph.StartsWith("*"))
                result.Add(new
                {
                    Type = "ListItem",
                    Text = paragraph
                });
            else
                result.Add(new
                {
                    Type = "Paragraph",
                    Text = paragraph
                });

        return result;
    }
}