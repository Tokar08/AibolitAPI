using AibolitAPI.Interfaces;

public class OpenAIDiseaseSearchProvider : IDiseaseSearchProvider
{
    public async Task<object> SearchAsync(string term)
    {
        return new object();
    }
}