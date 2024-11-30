namespace AibolitAPI.Interfaces;

public interface IDiseaseSearchProvider
{
    Task<object> SearchAsync(string term);
}