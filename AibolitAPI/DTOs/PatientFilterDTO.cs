namespace AibolitAPI.DTOs;

public class PatientFilterDTO
{
    public string? SearchTerm { get; set; }
    public string? Gender { get; set; }
    public string? City { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? MinBirthDate { get; set; }
    public DateTime? MaxBirthDate { get; set; }
}