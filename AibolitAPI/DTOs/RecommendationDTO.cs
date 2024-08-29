namespace AibolitAPI.DTOs;

public class RecommendationDTO
{
    public Guid Id { get; set; }
    public string PatientFullName { get; set; }
    public string DoctorFullName { get; set; }
    public DateTime RecommendationDate { get; set; }
    public string Content { get; set; }
    public bool IsActive { get; set; }
}