namespace AibolitAPI.DTOs;

public class RecommendationFilterDTO
{
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public string? ContentSearch { get; set; }
    public bool? SortByDescending { get; set; }
}