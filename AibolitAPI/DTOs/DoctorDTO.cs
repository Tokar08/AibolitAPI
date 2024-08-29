namespace AibolitAPI.DTOs;

public class DoctorDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Specialization { get; set; }
    public string HospitalTitle { get; set; }
    public int VisitCount { get; set; }
    public bool IsActive { get; set; }
}