namespace AibolitAPI.DTOs;

public class PrescriptionFilterDTO
{
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public string? MedicationName { get; set; }
    public bool? SortByDescending { get; set; }
}