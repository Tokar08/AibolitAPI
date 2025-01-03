namespace AibolitAPI.DTOs;

public class DoctorFilterDTO
{
    public string? SearchTerm { get; set; }
    public string? SpecializationTitle { get; set; }
    public string? HospitalTitle { get; set; }
    public string? Gender { get; set; }
    public int? MinYearsOfExperience { get; set; }
    public int? MaxYearsOfExperience { get; set; }
}